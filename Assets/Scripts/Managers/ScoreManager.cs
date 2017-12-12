using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour {
	//our score
	public int m_score = 0;

	// the number of lines we need to get to the next level
	int m_lines;

	// our current level
	public int m_level = 1;

	// base number of lines needed to clear a level
	public int m_linesPerLevel = 5;

	// text component for our Lines UI
	public Text m_linesText;

	// text component for our Level UI
	public Text m_levelText;

	// text component for our Score UI
	public Text m_scoreText;

	// minimum number of lines we can clear if we do indeed clear any lines
	const int m_minLines = 1;

	// maximum number of lines we can clear if we do indeed clear any lines
	const int m_maxLines = 4;

	//
	public bool didLevelUp = false; 

	// update the user interface
	void UpdateUIText()
	{
		if (m_linesText)
		{
			m_linesText.text = m_lines.ToString();
		}

		if (m_levelText)
		{
			m_levelText.text = m_level.ToString();
		}

		if (m_scoreText)
		{
			m_scoreText.text = PadZero(m_score,5);
		}
	}

	// handle scoring
	public void ScoreLines(int n)
	{
		// flag to GameController that we leveled up
		didLevelUp = false;

		// clamp this between 1 and 4 lines
		n = Mathf.Clamp(n,m_minLines,m_maxLines);

		// adds to our score depending on how many lines we clear
		switch (n)
		{
			case 1:
				m_score += 40 * m_level;
				break;
			case 2:
				m_score += 100 * m_level;
				break;
			case 3:
				m_score += 300 * m_level;
				break;
			case 4:
				m_score += 1200 * m_level;
				break;
		}

		// reduce our current number of lines needed for the next level
		m_lines -= n;

		// if we finished our lines, then level up
		if (m_lines <= 0)
		{
			LevelUp();
		}

		// update the user interface
		UpdateUIText();
	}

	// start our level and lines -- in the future we might start at a different level than 1 for increased difficulty
	public void Reset()
	{
		m_level = 1;
		m_lines = m_linesPerLevel * m_level;
		UpdateUIText();
	}

	// increments our level
	public void LevelUp()
	{
		m_level++;
		m_lines = m_linesPerLevel* m_level;
		didLevelUp = true;
	}

	void Start () 
	{
		Reset();
	}

	// returns a string padded to a certain number of places
	string PadZero(int n,int padDigits)
	{
		string nStr = n.ToString();

		while (nStr.Length < padDigits)
		{
			nStr = "0" + nStr;
		}
		return nStr;
	}



}
