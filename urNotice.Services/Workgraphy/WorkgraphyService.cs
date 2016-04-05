using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using urNotice.Common.Infrastructure.Common.Constants;
using urNotice.Common.Infrastructure.Model.urNoticeModel.AssetClass;
using urNotice.Common.Infrastructure.Model.urNoticeModel.DynamoDb;
using urNotice.Common.Infrastructure.Model.Workgraphy.Model;
using urNotice.Common.Infrastructure.Session;
using urNotice.Services.GraphDb.GraphDbContract;
using urNotice.Services.NoSqlDb.DynamoDb;
using urNotice.Services.Solr.SolrUser;
using urNotice.Services.Solr.SolrWorkgraphy;

namespace urNotice.Services.Workgraphy
{
    public class WorkgraphyService : IWorkgraphyService
    {
        protected ISolrWorkgraphy _solrWorkgraphyModel;
        protected ISolrUser _solrUserModel;
        protected IGraphDbContract _graphDbContractModel;
        protected IDynamoDb _dynamoDbModel;
        protected bool IsValidationEmailRequired;
        protected ResponseModel<StoryPostResponse> response;

        public WorkgraphyService(ISolrUser solrUserModel,ISolrWorkgraphy solrWorkgraphyModel, IDynamoDb dynamoDbModel, IGraphDbContract graphDbContractModel)
        {
            this._solrWorkgraphyModel = solrWorkgraphyModel;
            this._solrUserModel = solrUserModel; 
            this._dynamoDbModel = dynamoDbModel;
            this._graphDbContractModel = graphDbContractModel;
            this.IsValidationEmailRequired = false;
            response = new ResponseModel<StoryPostResponse>();
        }

        public ResponseModel<StoryPostResponse> PublishNewWorkgraphy(urNoticeSession session, StoryPostRequest req)
        {
            req.Data.email = req.Data.email.ToLower();

            var validateInputResponse = ValidateInputResponse(req);
            if (validateInputResponse.AbortProcess) return validateInputResponse;

            if (session.UserVertexId == CommonConstants.NA)
            {
                session = CheckIfAnonymousUserAlreadyPresentInSystem(req);
            }

            return SaveUserToDb(session,req);
        }

        protected ResponseModel<StoryPostResponse> SaveUserToDb(urNoticeSession session, StoryPostRequest workgraphy)
        {


            bool graphDbDataInserted = true;
            bool dynamoDbDataInserted = true;
            bool solrDbDataInserted = true;

            //TODO: need to implement try catch for roll back if any exception occurs
            try
            {
                var workgraphyVertexIdInfo = _graphDbContractModel.InsertNewWorkgraphyInGraphDb(workgraphy);
                workgraphy.vertexId = workgraphyVertexIdInfo[TitanGraphConstants.Id];
                try
                {
                    var orbitPageWorkgraphy = GenerateOrbitPageWorkgraphyObject(session, workgraphy);
                    orbitPageWorkgraphy.workgraphyVertexId = workgraphy.vertexId;

                    _dynamoDbModel.UpsertOrbitPageWorkgraphy(orbitPageWorkgraphy);
                    try
                    {
                        _solrWorkgraphyModel.InsertNewWorkgraphy(orbitPageWorkgraphy, false);
                    }
                    catch (Exception)
                    {
                        solrDbDataInserted = false;
                    }

                }
                catch (Exception)
                {
                    dynamoDbDataInserted = false;
                }
            }
            catch (Exception)
            {
                graphDbDataInserted = false;
            }

            if (!graphDbDataInserted)
            {
                //graph db insertion failed..
                OrbitPageResponseModel.SetInternalServerError("GraphDb Data insertion Failed.", new StoryPostResponse());
            }
            else if (!dynamoDbDataInserted)
            {
                //dynamoDb insertion failed..
                OrbitPageResponseModel.SetInternalServerError("DynamoDb Data insertion Failed.", new StoryPostResponse());
            }
            else if (!solrDbDataInserted)
            {
                //solr db insertion failed..
                OrbitPageResponseModel.SetInternalServerError("SolrDb Data insertion Failed.", new StoryPostResponse());
            }

            response.Status = 200;
            response.Message = "Created Successfully.";
            return response;
        }

        protected OrbitPageWorkgraphy GenerateOrbitPageWorkgraphyObject(urNoticeSession session, StoryPostRequest req)
        {
            //TODO:Create workgraphy vertex;

            var orbitPageWorkgraphy = new OrbitPageWorkgraphy()
            {
                companyName = req.Data.companyName,
                companyVertexId = req.Data.companyVertexId,
                createdByEmail = req.Data.email,
                createdByVertexId = session.UserVertexId,
                designation = req.Data.designation,
                designationVertexId = req.Data.designationVertexId,
                heading = req.Data.heading,
                shareAnonymously = req.Data.shareAnonymously,
                story = req.Data.story,
                
            };

            var images = new List<String>();
            foreach (var image in req.ImgurList)
            {
                images.Add(image.data.link);
            }

            foreach (var location in req.location)
            {
                if (location.types != null && location.types.Count > 0)
                {
                    switch (location.types[0])
                    {
                        case "sublocality_level_1":
                            orbitPageWorkgraphy.sublocality = location.long_name;
                            break;
                        case "locality":
                            orbitPageWorkgraphy.city = location.long_name;
                            break;
                        case "administrative_area_level_2":
                            orbitPageWorkgraphy.district = location.long_name;
                            break;
                        case "administrative_area_level_1":
                            orbitPageWorkgraphy.state = location.long_name;
                            break;
                        case "country":
                            orbitPageWorkgraphy.country = location.long_name;
                            break;
                        case "postal_code":
                            orbitPageWorkgraphy.postal_code = location.long_name;
                            break;

                    }
                }
                
            }

            orbitPageWorkgraphy.ImagesUrl = images;

            return orbitPageWorkgraphy;
        }

        protected urNoticeSession CheckIfAnonymousUserAlreadyPresentInSystem(StoryPostRequest req)
        {
            var solrUserEmail = _solrUserModel.GetPersonData(req.Data.email, null, null, null, false);
            urNoticeSession response;
            
            if (solrUserEmail != null)
            {                
                response = new urNoticeSession(solrUserEmail.Email, solrUserEmail.VertexId);
            }
            else
            {                
                response = new urNoticeSession(req.Data.email, CommonConstants.AnonymousUserVertex); // TODO: use anonymous user vertex.
            }
            return response;
        }

        protected ResponseModel<StoryPostResponse> ValidateInputResponse(StoryPostRequest req)
        {
            var response = new ResponseModel<StoryPostResponse>();
            response.AbortProcess = false;
            return response;
        }
    }
}
