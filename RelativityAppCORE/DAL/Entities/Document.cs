using System.Collections.Generic;

namespace RelativityAppCore.DAL.Entities
{
    public class Document : Artifact
    {
        public Document()
        {

        }
        public Document(int artifact): base(artifact)
        {
            ArtifactId = artifact;
        }
        public IEnumerable<Comment> Comments { get; set; }
        public string RelativityNativeFileLocation { get; set; }
        public int ParentArtifactId { get; set; }
        public int amountComments { get; set; }
    }
}
