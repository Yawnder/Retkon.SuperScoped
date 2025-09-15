namespace Retkon.SuperScoped.Tests.TestClasses;
internal class MyServiceChild
{
    private readonly List<string> result;

#pragma warning disable IDE0052 // Remove unread private members
    private readonly MyScope myScope;
#pragma warning restore IDE0052 // Remove unread private members

    public MyServiceChild(
        List<string> result,
        MyScope myScope)
    {
        this.result = result;

        this.result.Add("MyServiceChild::ctor");
        this.myScope = myScope;
    }

    public void Poke()
    {
        this.result.Add("MyServiceChild::Poke");
    }

}
