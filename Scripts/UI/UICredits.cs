namespace Template;

public partial class UICredits : Node
{
    private VBoxContainer vbox;
    private Button btnPause;
    private Button btnSpeed;
    private bool paused;
    private float speed = 1.0f;

    public override void _Ready()
    {
        btnPause = GetNode<Button>("HBox/Pause");
        btnSpeed = GetNode<Button>("HBox/Speed");

        vbox = new VBoxContainer
        {
            SizeFlagsVertical = Control.SizeFlags.ShrinkBegin
        };

        var file = FileAccess.Open("res://credits.txt", FileAccess.ModeFlags.Read);

        while (!file.EofReached())
        {
            var line = Tr(file.GetLine());

            var translatedLine = "";

            foreach (var word in line.Split(' '))
                translatedLine += Tr(word) + " ";

            if (translatedLine.Contains("http"))
                AddTextWithLink(translatedLine);
            else
                if (string.IsNullOrWhiteSpace(translatedLine))
                    vbox.AddChild(new GPadding(0, 10));
                else
                    vbox.AddChild(new GLabel(translatedLine));
        } 

        file.Close();

        AddChild(vbox);

        vbox.Position = new Vector2(
            DisplayServer.WindowGetSize().X / 2 - vbox.Size.X / 2,
            DisplayServer.WindowGetSize().Y);
    }

    public override void _PhysicsProcess(double delta)
    {
        var pos = vbox.Position;
        pos.Y -= speed;
        vbox.Position = pos;

        if (pos.Y <= -vbox.Size.Y)
        {
            AudioManager.PlayMusic(Music.Menu);
            SceneManager.SwitchScene("main_menu");
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("ui_cancel"))
        {
            AudioManager.PlayMusic(Music.Menu);
            SceneManager.SwitchScene("main_menu");
        }
    }

    private void AddTextWithLink(string text)
    {
        var indexOfHttp = text.IndexOf("http");

        var textDesc = text.Substring(0, indexOfHttp);
        var textLink = text.Substring(indexOfHttp);

        var hbox = new HBoxContainer
        {
            SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter
        };

        var labelText = new GLabel(textDesc);
        var btnLink = new GLinkButton(textLink);

        hbox.AddChild(labelText);
        hbox.AddChild(btnLink);

        vbox.AddChild(hbox);
    }

    private void _on_pause_pressed()
    {
        paused = !paused;

        if (paused)
        {
            SetPhysicsProcess(false);
            btnPause.Text = "Resume";
        }
        else
        {
            SetPhysicsProcess(true);
            btnPause.Text = "Pause";
        }
    }

    private void _on_speed_pressed()
    {
        var numSpeeds = 3;

        for (int i = 1; i < numSpeeds; i++)
            if (speed == i)
            {
                btnSpeed.Text = $"{i + 1}.0x";
                speed = i + 1;
                return;
            }

        if (speed == numSpeeds)
        {
            btnSpeed.Text = "1.0x";
            speed = 1;
        }
    }
}
