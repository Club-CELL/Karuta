using UnityEngine;
using UnityEngine.SceneManagement;

public class Retour : ScaleMoveButton
{
    public override void Execute()
	{
		pos = transform.position;
		if (pos.x - x0 > finX)
		{
			pos = new Vector2 (pos.x - speedX, pos.y);
			transform.position = pos;

		} else
		{
            selected = false;
            Global.Restart();
            SceneManager.LoadScene("MainMenu");
        }
	}
}
