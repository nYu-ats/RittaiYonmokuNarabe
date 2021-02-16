using Cysharp.Threading.Tasks;
using FirebaseChildKey;

interface ISetUserName{
    UniTask SetUserName(string setUserName);
}

interface IUserNameValidation{
    UniTask<bool> UserNameValidation(string userName);
}

public class FirebaseUserRgisterFunc : BaseFirebaseFunc, IUserNameValidation, ISetUserName
{
    public async UniTask<bool> UserNameValidation(string userName){
        try{
            return await reference.Child("user").Child(userName).GetValueAsync().ContinueWith(task => {
                if(task.Result.GetRawJsonValue() == null){
                    return true;
                }
                else{
                    return false;
                }
            });
        }
        catch{
            //エラーが発生した場合は、ユーザー名の重複登録を防ぐためfalseを返して
            //ユーザー名の再入力を促す
            return false;
        }
    }

    //ユーザー名をFirebaseへ格納する
    public async UniTask SetUserName(string setUserName){
        await reference.Child(GetKey.UserKey).Child(setUserName).SetValueAsync(true);
    }
}
