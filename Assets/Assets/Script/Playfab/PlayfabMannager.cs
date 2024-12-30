using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.UI;

public class PlayfabMannager : MonoBehaviour
{
    public CharacterSave[] characterSave;
    public TextMeshProUGUI messageText;
    public TMP_InputField emailInput_Register;
    public TMP_InputField passwordInput_Register;
    public TMP_InputField emailInput_Login;
    public TMP_InputField passwordInput_Login;


    public GameObject rowObject;
    public Transform rowsParent;

    //public GameObject nameWindow;
   // public GameObject leaderBoardWindow;

   // public GameObject nameError;
    //public TMP_InputField nameInput;
    // Start is called before the first frame update
    void Start()
    {
        Login();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true,
            /*
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
            */
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);
    }

    void OnSuccess(LoginResult result)
    {
        Debug.Log("Successful login/account create!");
        //string name = null;
      //  if(result.InfoResultPayload.PlayerProfile != null)
       // name = result.InfoResultPayload.PlayerProfile.DisplayName;
        //GetCharacter();

        /*
        if(name == null)
            nameWindow.SetActive(true);
        else
        {
            leaderBoardWindow.SetActive(true);
        }
        */
    }

    public void SendLeaderBoard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "PTIT Leaderboard",
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderBoardUpdated, OnError);
    }

    void OnError(PlayFabError error)
    {
        messageText.text = error.ErrorMessage;
        Debug.Log(error.GenerateErrorReport());
    }

    void OnLeaderBoardUpdated(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Succcessful LeaderBoard sent");
    }

    public void GetLeaderBoard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "PTIT Leaderboard",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderBoardGet, OnError);
    }

    void OnLeaderBoardGet(GetLeaderboardResult result)
    {
        foreach(Transform item in rowsParent)
        {
            Destroy(item.gameObject);
        }

        foreach(var item in result.Leaderboard)
        {
            GameObject newGo = Instantiate(rowObject, rowsParent);
            TextMeshProUGUI[] texts = newGo.GetComponentsInChildren<TextMeshProUGUI>();
            texts[0].text = (item.Position + 1).ToString();
            texts[1].text = item.DisplayName;
            texts[2].text = item.StatValue.ToString();

            Debug.Log(item.Position + " " + item.PlayFabId + " " + item.StatValue);
        }
    }

    void OnDataSend(UpdateUserDataResult result)
    {
        Debug.Log("Successful user data send");
    }

    public void SaveCharacter()
    {
        List<CharacterInformation> characterInformation = new List<CharacterInformation>();
        foreach(var item in characterSave)
        {
            characterInformation.Add(item.ReturnClass());
        }
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {
                    "Characters", JsonConvert.SerializeObject(characterInformation)
                }
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSend, OnError);
    }


    public void GetCharacter()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnCharactersDataRecived, OnError);
    }

    void OnCharactersDataRecived(GetUserDataResult result)
    {
        Debug.Log("Recieved characters data!");
        if (result.Data != null && result.Data.ContainsKey("Characters"))
        {
            List<CharacterInformation> characterInformation = JsonConvert.DeserializeObject<List<CharacterInformation>>(result.Data["Characters"].Value);
            for (int i = 0; i < characterSave.Length; i++)
            {
                characterSave[i].SetUI(characterInformation[i]);    
            }
        }
    }


    public void RegisterButton()
    {
        if(passwordInput_Register.text.Length < 6)
        {
            messageText.text = "Mật khẩu quá ngắn";
            return;
        }
        var request = new RegisterPlayFabUserRequest
        {
            Email = emailInput_Register.text,
            Password = passwordInput_Register.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnError);
    }

    public void LoginButton()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = emailInput_Login.text,
            Password = passwordInput_Login.text
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnSuccess, OnError );
    }

    void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        messageText.text = "Đăng kí thành công";
    }



    /*
    public void SubmitNameButton()
    {
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = nameInput.text,
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
    }

    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Update name");
       // leaderBoardWindow.SetActive(true);
    }
    */
}
