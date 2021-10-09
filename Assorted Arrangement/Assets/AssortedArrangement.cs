using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using Grid = GridInfo;

public class AssortedArrangement : MonoBehaviour {

   public KMBombInfo Bomb;
   public KMAudio Audio;
   public KMSelectable[] Buttons;
   public SpriteRenderer[] DisplayedObjects;
   public Sprite[] Objects;

   int ChosenSquareRow;
   int ChosenSquareColumn;
   int LetterPart;
   int NumericalPart;

   readonly string LetterCoordinates = "ABCDEFGHIJKLMNOPQRS";
   readonly string[] LogNames = { "banana", "bomb", "candy cane", "shot glass", "chips", "peanut", "saxophone", "trumpet" };

   static int ModuleIdCounter = 1;
   int ModuleId;
   private bool ModuleSolved;

   void Awake () {
      ModuleId = ModuleIdCounter++;

      foreach (KMSelectable Button in Buttons) {
         Button.OnInteract += delegate () { ButtonPress(Button); return false; };
      }
   }

   void Start () {
      //ChosenSquare = Random.Range(0, Grid.Grid.Length);
      ChosenSquareRow = Random.Range(0, 19);
      ChosenSquareColumn = Random.Range(0, 19);
      DisplayedObjects[0].GetComponent<SpriteRenderer>().sprite = Objects[Grid.Grid[ChosenSquareColumn][ChosenSquareRow]];
      DisplayedObjects[1].GetComponent<SpriteRenderer>().sprite = Objects[Grid.Grid[ChosenSquareColumn][ChosenSquareRow + 1]];
      DisplayedObjects[2].GetComponent<SpriteRenderer>().sprite = Objects[Grid.Grid[ChosenSquareColumn + 1][ChosenSquareRow]];
      DisplayedObjects[3].GetComponent<SpriteRenderer>().sprite = Objects[Grid.Grid[ChosenSquareColumn + 1][ChosenSquareRow + 1]];
      Debug.LogFormat("[The Assorted Arrangement #{0}] The shown items in reading order are: {1}, {2}, {3}, {4}.", ModuleId, LogNames[Grid.Grid[ChosenSquareColumn][ChosenSquareRow]], LogNames[Grid.Grid[ChosenSquareColumn][ChosenSquareRow + 1]], LogNames[Grid.Grid[ChosenSquareColumn + 1][ChosenSquareRow]], LogNames[Grid.Grid[ChosenSquareColumn + 1][ChosenSquareRow + 1]]);
      Debug.LogFormat("[The Assorted Arrangement #{0}] The coordinate is {1}{2}.", ModuleId, LetterCoordinates[ChosenSquareRow], (ChosenSquareColumn + 1).ToString());
      //Debug.Log(ChosenSquare);
      //Debug.Log(Grid.Grid[ChosenSquare][0].ToString() + Grid.Grid[ChosenSquare][1].ToString() + Grid.Grid[ChosenSquare][2].ToString() + Grid.Grid[ChosenSquare][3].ToString());
   }

   void ButtonPress (KMSelectable Button) {
      Button.AddInteractionPunch();
      for (int i = 0; i < 4; i++) {
         if (Button == Buttons[i]) {
            switch (i) {
               case 0:
                  Audio.PlaySoundAtTransform("Noise", Button.transform);
                  LetterPart++;
                  LetterPart %= 19;
                  break;
               case 1:
                  Audio.PlaySoundAtTransform("OtherNoise", Button.transform);
                  //Debug.Log(LetterPart * 19 + NumericalPart);
                  if (LetterPart == ChosenSquareRow && NumericalPart == ChosenSquareColumn) {
                     if (!ModuleSolved) {
                        GetComponent<KMBombModule>().HandlePass();
                        Debug.LogFormat("[The Assorted Arrangement #{0}] The coordinate submitted was the same as the given one. Module disarmed.", ModuleId);
                        ModuleSolved = true;
                     }
                  }
                  else if (!ModuleSolved) {
                     GetComponent<KMBombModule>().HandleStrike();
                     Debug.LogFormat("[The Assorted Arrangement #{0}] The coordinate you submitted was {1}{2}.", ModuleId, LetterCoordinates[LetterPart], (NumericalPart + 1).ToString());
                     LetterPart &= 0;
                     NumericalPart &= 0;
                  }
                  break;
               case 2:
                  Audio.PlaySoundAtTransform("Noise", Button.transform);
                  LetterPart &= 0;
                  NumericalPart &= 0;
                  break;
               case 3:
                  Audio.PlaySoundAtTransform("Noise", Button.transform);
                  NumericalPart++;
                  NumericalPart %= 19;
                  break;
            }
         }
      }
   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} TL/TR/BL/BR # to press a button that many times. The number is optional.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
      string[] Parameters = Command.Trim().ToUpper().Split(' ');
      yield return null;
      for (int i = 0; i < Parameters.Length; i++) {
         if (Parameters[i].Length > 20) {
            yield return "sendtochaterror Stop trying to get Deaf to fix a TP bug, dick.";
            yield break;
         }
      }
      if (Parameters.Length > 2) {
         yield return "sendtochaterror I don't understand!";
      }
      else if (Parameters.Length == 2) {
         if (!Parameters[1].Any(x => "1234567890".Contains(x)) || Parameters[1].Length > 100 || Parameters[1] == "0") {
            yield return "sendtochaterror I don't understand!";
         }
         else {
            switch (Parameters[0]) {
               case "TL":
                  for (int i = 0; i < int.Parse(Parameters[1]); i++) {
                     Buttons[0].OnInteract();
                     yield return new WaitForSecondsRealtime(0.1f);
                  }
                  break;
               case "TR":
                  for (int i = 0; i < int.Parse(Parameters[1]); i++) {
                     Buttons[1].OnInteract();
                     yield return new WaitForSecondsRealtime(0.1f);
                  }
                  break;
               case "BL":
                  for (int i = 0; i < int.Parse(Parameters[1]); i++) {
                     Buttons[2].OnInteract();
                     yield return new WaitForSecondsRealtime(0.1f);
                  }
                  break;
               case "BR":
                  for (int i = 0; i < int.Parse(Parameters[1]); i++) {
                     Buttons[3].OnInteract();
                     yield return new WaitForSecondsRealtime(0.1f);
                  }
                  break;
               default:
                  yield return "sendtochaterror I don't understand!";
                  break;
            }
         }
      }
      else {
         switch (Parameters[0]) {
            case "TL":
               Buttons[0].OnInteract();
               break;
            case "TR":
               Buttons[1].OnInteract();
               break;
            case "BL":
               Buttons[2].OnInteract();
               break;
            case "BR":
               Buttons[3].OnInteract();
               break;
            default:
               yield return "sendtochaterror I don't understand!";
               break;
         }
      }
   }

   IEnumerator TwitchHandleForcedSolve () {
      yield return null;
      while (ChosenSquareRow != LetterPart) {
         Buttons[0].OnInteract();
         yield return new WaitForSecondsRealtime(0.1f);
      }
      while (ChosenSquareColumn != NumericalPart) {
         Buttons[3].OnInteract();
         yield return new WaitForSecondsRealtime(0.1f);
      }
      Buttons[1].OnInteract();
   }
}
