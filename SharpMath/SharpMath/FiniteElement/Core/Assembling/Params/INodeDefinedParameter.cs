namespace SharpMath.FiniteElement.Core.Assembling.Params;

public interface INodeDefinedParameter<out T>
{
    public T Get(int nodeIndex);
}