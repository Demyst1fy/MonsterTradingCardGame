namespace SWEN1.MTCG.Server.JSONClasses
{
    public class UserinfoJSON
    {
        public string Name { get; }
        public string Bio { get; }
        public string Image { get; }
        
        public UserinfoJSON(string name, string bio, string image)
        {
            Name = name;
            Bio = bio;
            Image = image;
        }
    }
}