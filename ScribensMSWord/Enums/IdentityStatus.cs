using ScribensMSWord.Attributes;

namespace ScribensMSWord.Enums
{
    public enum IdentityStatus
    {
        True, //User has a Premium account
        TimeExpired, //User has a expired Premium account
        InscriptionSimple, //User does not have a Premium account

        [ResourceMessage("UserInfoPane.Message.SessionActive")]
        SessionActive, //This Premium account is in used at another machine

        [ResourceMessage("UserInfoPane.Message.InvalidAccount")]
        NotExistingId, //Id is not existing

        [ResourceMessage("UserInfoPane.Message.InvalidAccount")]
        InvalidPassword, //Invalid password,

        [ResourceMessage("UserInfoPane.Message.InactiveAccount")]
        InactivatedAccount, //Inactivated account
    }
}
