namespace SharpMath.FiniteElement.Core.Assembling.Boundary.First;

//public record struct FirstConditions(int[] NodesIndexes, double[] Values);
public record struct FirstCondition(int NodeIndex, double Value);