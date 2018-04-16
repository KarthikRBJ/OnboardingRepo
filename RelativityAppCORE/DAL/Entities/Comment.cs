using System.Collections.Generic;

namespace RelativityAppCore.DAL.Entities
{
    public class Comment : Artifact
    {
        public Comment()
        {

        }
        public Comment(int artifact): base(artifact)
        {
            ArtifactId = artifact;
        }
        public IEnumerable<Comment> CommentChilds { get; set; }
        public readonly string COMMENT = "338C23DE-21B1-49C1-A8B9-78F5CD742318";
        public readonly string ARTIFACT_TYPE = "3136AA28-7D29-4164-A928-CF2272197090";
        public readonly string SINGLE_CHOICE_FIELD = "F0D53EE2-3AD2-43ED-B68F-397849A17F89";
        public readonly string IMPROVEMENT_TYPER_CHOICE_FIELD = "17A7D9AC-289B-474F-8971-ED9E7F35BDA9";
        public readonly string CORRECTION_TYPER_CHOICE_FIELD = "FDE7E280-CF63-4BE6-A2CD-44DE65360BAE";
        public readonly string ERROR_TYPER_CHOICE_FIELD = "73DF39C3-3E97-4584-9AA4-03821A9D8AD2";
        public readonly string REVIEW_TYPER_CHOICE_FIELD = "52BCF68E-6D78-42C4-B6D7-1F42CA655DA3";
        public readonly string RELATED_COMMENT_FIELD = "8A9383C2-DE31-4B99-B31C-32FC4EA560EA";
        public readonly string COMMENT_IMAGE_FIELD = "D6F64A63-9619-405D-95E0-9B2E5556AE57";
        public readonly string COMMENT_THUMBNAIL_FIELD = "241FD1B9-85B4-4251-BE46-93D39EFC3616";

        public string imageBase64 { get; set; }
        public int ParentCommentId { get; set; }
        public string Type { get; set; }
        public string TypeChoosed { get; set; }
    }
}
