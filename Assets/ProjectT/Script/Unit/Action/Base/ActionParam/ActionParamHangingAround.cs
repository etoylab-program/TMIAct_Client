
public class ActionParamHangingAround : IActionBaseParam
{
    public float minDuration { get; private set; }
    public float maxDuration { get; private set; }
    public eActionCommand distActionCommand { get; private set; }
    public bool randomCheckDist { get; private set; }
    public eDirection[] directions { get; private set; }
    public bool turn { get; private set; }


    public ActionParamHangingAround(float minDuration, float maxDuration, eActionCommand distActionCommand, bool randomCheckDist,
                                    eDirection[] directions, bool turn)
    {
        this.minDuration = minDuration;
        this.maxDuration = maxDuration;
        this.distActionCommand = distActionCommand;
        this.randomCheckDist = randomCheckDist;
        this.directions = directions;
        this.turn = turn;
    }
}
