using System;

namespace RelativityAppCore.DAL.Entities
{
    public class Artifact
    {
        public Artifact()
        {

        }

        public Artifact(int artifactId) : this()
        {
            ArtifactId = artifactId;
        }

        public int ArtifactId { get; set; }
        public string Name { get; set; }

        public Artifact CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public Artifact LastModifiedBy { get; set; }
        public string LastModifiedOn { get; set; }
    }
}
