namespace day15;

internal class Solver
{
    private readonly Ingredient[] _ingredients;
    private readonly int _totalQuantity;

    public Solver(Ingredient[] ingredients, int totalQuantity)
    {
        _ingredients = ingredients;
        _totalQuantity = totalQuantity;
    }

    public int Solve(int? targetCalories)
    {
        var quantities = new int[_ingredients.Length];

        int maxScore = 0;
        
        while (true)
        {
            if (IsValidCombination(quantities))
            {
                (int score, int calories) = CalculateScore(quantities);

                if (score > maxScore && (targetCalories is null || targetCalories == calories))
                {
                    maxScore = score;
                }
            }
            
            if (!NextCombination(quantities))
                break;
        }
        
        return maxScore;
    }

    private (int,int) CalculateScore(int[] quantities)
    {
        const int numEffects = 5; // ^^

        var effectScores = new int[numEffects];

        for (int ingredientIdx = 0; ingredientIdx < quantities.Length; ingredientIdx++)
        {
            var ingredient = _ingredients[ingredientIdx];
            var quantity = quantities[ingredientIdx];

            for (int effectIdx = 0; effectIdx < numEffects; effectIdx++)
                effectScores[effectIdx] += ingredient.Effects[effectIdx].Value * quantity;
        }

        int totalScore = 1;
        for (int effectIdx = 0; effectIdx < numEffects - 1; effectIdx++)
            totalScore *= Math.Max(0, effectScores[effectIdx]);

        int totalCalories = effectScores.Last();

        return (totalScore, totalCalories);
    }

    private bool NextCombination(int[] quantities)
    {
        for (int i = 0; i < quantities.Length; i++)
        {
            quantities[i]++;

            if (quantities[i] > _totalQuantity)
            {
                quantities[i] = 0;

                if (i == quantities.Length - 1)
                    return false;
            }
            else
            {
                break;
            }
        }

        return true;
    }

    private bool IsValidCombination(int[] quantities)
    {
        return quantities.Sum() == _totalQuantity;
    }
}