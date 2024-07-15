using UnityEditor;

namespace Yohash.React.Editor
{
  public static class GenerateAll
  {
    [MenuItem("Yohash/React/Generate All Code", false, 200)]
    public static void GenerateCode()
    {
      StateGenerator.GenerateMainState();
      //MiddlewaresGenerator.GenerateMiddlewares();
      PropsGenerator.GeneratePropsMethods();
    }
  }
}
