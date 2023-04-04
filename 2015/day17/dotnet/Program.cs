
const string inputFile = "../../../input.txt";

int[] buckets = File
    .ReadLines(inputFile)
    .Select(Int32.Parse)
    .ToArray();

// Part1
var solutions = new HashSet<SolutionIndices>();
Visit(Array.Empty<int>());
Console.WriteLine($"TotalSolutions: {solutions.Count}");

// Part2
int minNumberOfContainers = solutions.Select(x => x.Indices.Length).Min();
Console.WriteLine($"MinNumberOfContainers: {minNumberOfContainers}");
int numberOfSolutionsWithMinContainers = solutions.Count(x => x.Indices.Length == minNumberOfContainers);
Console.WriteLine($"numberOfSolutionsWithMinContainers: {numberOfSolutionsWithMinContainers}");

void Visit(int[] usedBucketsIndices)
{
    var usedBuckets = usedBucketsIndices.Select(idx => buckets[idx]).ToArray();
    int sum = usedBuckets.Sum();
    
    switch (sum)
    {
        case 150:
            solutions.Add(new SolutionIndices(usedBucketsIndices));
            return;
        
        case > 150:
            return;
    }

    int[] bucketIndices = Enumerable.Range(0, buckets.Length).ToArray();
    int[] availableBucketIndices = bucketIndices.Except(usedBucketsIndices).ToArray();
    
    if (availableBucketIndices.Length != bucketIndices.Length - usedBucketsIndices.Length)
        throw new Exception("kaputt");
    
    for (int i = 0; i < availableBucketIndices.Length; i++)
    {
        int currentBucketIndex = availableBucketIndices[i];
        int[] newUsedBucketsIndices = usedBucketsIndices.Concat(new[] { currentBucketIndex }).ToArray();
        Visit(newUsedBucketsIndices);
    }
}

internal class SolutionIndices
{
    public int[] Indices { get; }

    public SolutionIndices(int[] indices)
    {
        if (indices.Length != indices.Distinct().Count())
            throw new Exception("invalid solution index");
        
        Indices = indices.OrderBy(i => i).ToArray();
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;

        return obj is SolutionIndices other &&
               Enumerable.SequenceEqual(this.Indices, other.Indices);
    }

    public override int GetHashCode()
    {
        int hc = 123;
        for (int i = 0; i < Indices.Length; i++)
            hc = HashCode.Combine(hc, Indices[i]);
        return hc;
    }
}
