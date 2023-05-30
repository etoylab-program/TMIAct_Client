
public class ActionParamAI : IActionBaseParam
{
    public eDirection       Direction       { get; set; }
    public float            minValue        { get; private set; } = 0.0f;
    public float            maxValue        { get; private set; } = 0.0f;
    public eActionCommand   distCommand     { get; private set; } = eActionCommand.None;
    public bool             posByTargetDir  { get; private set; } = false;
    public bool             LookAtByRotate  { get; private set; } = false;


    public ActionParamAI() {
	}

    public ActionParamAI(eDirection direction, float minValue, float maxValue, eActionCommand distCommand, bool posByTargetDir, bool lookAtByRotate)
    {
        Direction = direction;
        this.minValue = minValue;
        this.maxValue = maxValue;
        this.distCommand = distCommand;
        this.posByTargetDir = posByTargetDir;
        LookAtByRotate = lookAtByRotate;
    }
}
