namespace Yohash.React.Samples.BasicUi
{
  public struct TestStruct
  {
    public int Value;
    public string Name;
  }

  /// <summary>
  /// This state container is a placeholder so that various member data
  /// can be expressed and viewed in the editor state viewer tool. This
  /// can help test to ensure good viewing of all data types.
  /// </summary>
  public class ViewerToolTestState : StateContainer
  {
    public TestStruct? NullableTestStruct =
      new TestStruct()
      {
        Name = "TestName",
        Value = 1
      };

    public TestStruct? NullTestStruct = null;

    public int? TestInt = 1;

    protected override bool reduce(IAction action)
    {
      return false;
    }
  }
}