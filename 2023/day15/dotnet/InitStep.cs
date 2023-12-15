namespace day15;

public abstract record InitStep()
{
    public abstract void Execute(State state);
}

public record AddLensStep(string Label, int FocalLength) : InitStep
{
    public override void Execute(State state)
    {
        var box = state.Boxes[HolidayStringHelper.Hash(Label)];
        var newLens = new LensInBox(Label, FocalLength);

        var idx = box.Lenses.FindIndex(x => x.Label == Label);
        if (idx != -1)
            box.Lenses[idx] = newLens;
        else
            box.Lenses.Add(newLens);
    }
}

public record RemoveLensStep(string Label) : InitStep
{
    public override void Execute(State state)
    {
        state
            .Boxes[HolidayStringHelper.Hash(Label)]
            .Lenses
            .RemoveAll(x => x.Label == Label);
    }
}