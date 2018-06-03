using LogicSpawn.RPGMaker.Generic;

public class SubtitleTemplate
{
    public string Text;
    public float Duration;
    public string AudioPath;

    public SubtitleTemplate(string text, float duration = 3.0f, string audioPath = "")
    {
        Text = text;
        Duration = duration;
        AudioPath = audioPath;
    }

    public SubtitleTemplate()
    {
    }

    public static SubtitleTemplate Line(string text, float duration = 3.0f, string audioPath = "")
    {
        return new SubtitleTemplate(text, duration, audioPath);
    }
}