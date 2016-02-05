using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;

namespace ConsoleApplication1.DynamoDbModal
{
    [DynamoDBTable("DVD")]
    public class DVD
    {
        [DynamoDBHashKey]
        public string Title { get; set; }
        
        [DynamoDBRangeKey]
        public int ReleaseYear { get; set; }

        [DynamoDBProperty]
        public List<string> ActorNames { get; set; }
        public string Director { get; set; }
        public string Producer { get; set; }
        public override string ToString(){
        return string.Format(@"{0} – {1} Actors: {2}", Title, ReleaseYear, string.Join(", ", ActorNames.ToArray()));}
        }
    
}
