using System.IO;

class FileDebug {
  private static StreamWriter file;

  public static void Log(object thing) {
    if(file == null) {
      file = new System.IO.StreamWriter(@"log.txt");
    }
    file.WriteLine(thing.ToString());
  }
}
