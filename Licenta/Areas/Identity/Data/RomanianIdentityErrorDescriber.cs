using Microsoft.AspNetCore.Identity;

public class RomanianIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DefaultError()
    {
        return new IdentityError { Code = nameof(DefaultError), Description = "A apărut o eroare necunoscută." };
    }

    public override IdentityError ConcurrencyFailure()
    {
        return new IdentityError { Code = nameof(ConcurrencyFailure), Description = "Eroare de concurență optimistă, obiectul a fost modificat." };
    }

    public override IdentityError PasswordMismatch()
    {
        return new IdentityError { Code = nameof(PasswordMismatch), Description = "Parola este incorectă." };
    }

    public override IdentityError InvalidToken()
    {
        return new IdentityError { Code = nameof(InvalidToken), Description = "Token invalid." };
    }

    public override IdentityError LoginAlreadyAssociated()
    {
        return new IdentityError { Code = nameof(LoginAlreadyAssociated), Description = "Un utilizator cu acest login există deja." };
    }

    public override IdentityError InvalidUserName(string userName)
    {
        return new IdentityError { Code = nameof(InvalidUserName), Description = $"Numele de utilizator '{userName}' este invalid. Poate conține doar litere și cifre." };
    }

    public override IdentityError InvalidEmail(string email)
    {
        return new IdentityError { Code = nameof(InvalidEmail), Description = $"Adresa de email '{email}' este invalidă." };
    }

    public override IdentityError DuplicateUserName(string userName)
    {
        return new IdentityError { Code = nameof(DuplicateUserName), Description = $"Numele de utilizator '{userName}' este deja utilizat." };
    }

    public override IdentityError DuplicateEmail(string email)
    {
        return new IdentityError { Code = nameof(DuplicateEmail), Description = $"Adresa de email '{email}' este deja utilizată." };
    }

    public override IdentityError InvalidRoleName(string role)
    {
        return new IdentityError { Code = nameof(InvalidRoleName), Description = $"Numele rolului '{role}' este invalid." };
    }

    public override IdentityError DuplicateRoleName(string role)
    {
        return new IdentityError { Code = nameof(DuplicateRoleName), Description = $"Numele rolului '{role}' este deja utilizat." };
    }

    public override IdentityError UserAlreadyHasPassword()
    {
        return new IdentityError { Code = nameof(UserAlreadyHasPassword), Description = "Utilizatorul are deja o parolă configurată." };
    }

    public override IdentityError UserLockoutNotEnabled()
    {
        return new IdentityError { Code = nameof(UserLockoutNotEnabled), Description = "Blocarea contului nu este activată pentru acest utilizator." };
    }

    public override IdentityError UserAlreadyInRole(string role)
    {
        return new IdentityError { Code = nameof(UserAlreadyInRole), Description = $"Utilizatorul are deja rolul '{role}'." };
    }

    public override IdentityError UserNotInRole(string role)
    {
        return new IdentityError { Code = nameof(UserNotInRole), Description = $"Utilizatorul nu are rolul '{role}'." };
    }

    public override IdentityError PasswordTooShort(int length)
    {
        return new IdentityError { Code = nameof(PasswordTooShort), Description = $"Parola trebuie să aibă cel puțin {length} caractere." };
    }

    public override IdentityError PasswordRequiresNonAlphanumeric()
    {
        return new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "Parola trebuie să conțină cel puțin un caracter special (!@#$%^&* etc.)." };
    }

    public override IdentityError PasswordRequiresDigit()
    {
        return new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "Parola trebuie să conțină cel puțin o cifră (0-9)." };
    }

    public override IdentityError PasswordRequiresLower()
    {
        return new IdentityError { Code = nameof(PasswordRequiresLower), Description = "Parola trebuie să conțină cel puțin o literă mică (a-z)." };
    }

    public override IdentityError PasswordRequiresUpper()
    {
        return new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "Parola trebuie să conțină cel puțin o literă mare (A-Z)." };
    }

    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
    {
        return new IdentityError { Code = nameof(PasswordRequiresUniqueChars), Description = $"Parola trebuie să conțină cel puțin {uniqueChars} caractere unice." };
    }

    public override IdentityError RecoveryCodeRedemptionFailed()
    {
        return new IdentityError { Code = nameof(RecoveryCodeRedemptionFailed), Description = "Codul de recuperare nu a putut fi utilizat." };
    }
}