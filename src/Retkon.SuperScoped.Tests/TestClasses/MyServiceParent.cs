
namespace Retkon.SuperScoped.Tests.TestClasses;
internal class MyServiceParent
{
    private static int nextInstanceNumber = 0;
    public int InstanceNumber { get; } = nextInstanceNumber++;

    private readonly List<string> result;
    private readonly MyScope myScope;
    private readonly MyServiceChild myServiceChild;

    public MyServiceParent(
        List<string> result,
        MyScope myScope,
        MyServiceChild myServiceChild)
    {
        this.result = result;

        this.result.Add("MyServiceParent::ctor");
        this.myScope = myScope;
        this.myServiceChild = myServiceChild;
    }

    public void Poke()
    {
        this.result.Add("MyServiceParent::Poke");
    }

}
