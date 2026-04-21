namespace LerningApp.Common;

public static class EmailTemplates
{
    public static string EmailConfirmation(string callbackUrl)
    {
        return $@"
            <div style='font-family: Arial, sans-serif; max-width: 640px; margin: 0 auto; padding: 24px; color: #1f2937;'>
                <div style='background: linear-gradient(135deg, #ff9933, #ffb366); padding: 28px; border-radius: 20px 20px 0 0; color: white;'>
                    <h1 style='margin: 0; font-size: 28px;'>Confirm your email</h1>
                    <p style='margin: 8px 0 0; font-size: 16px;'>Welcome to DeutschBuddy!</p>
                </div>

                <div style='border: 1px solid #f3dfc9; border-top: 0; padding: 28px; border-radius: 0 0 20px 20px; background: #ffffff;'>
                    <p style='font-size: 16px; line-height: 1.7; margin-top: 0;'>
                        Thank you for creating your DeutschBuddy account. Please confirm your email address
                        so we can keep your account secure and help you start learning German.
                    </p>

                    <p style='text-align: center; margin: 30px 0;'>
                        <a href='{callbackUrl}'
                           style='display: inline-block; background: #ff9933; color: #ffffff; text-decoration: none; padding: 14px 24px; border-radius: 14px; font-weight: 700;'>
                            Confirm my email
                        </a>
                    </p>

                    <p style='font-size: 14px; line-height: 1.6; color: #6b7280;'>
                        If the button does not work, copy and paste this link into your browser:
                    </p>

                    <p style='word-break: break-all; font-size: 13px; color: #9a5515;'>
                        {callbackUrl}
                    </p>

                    <p style='font-size: 14px; line-height: 1.6; color: #6b7280; margin-bottom: 0;'>
                        If you did not create this account, you can safely ignore this email.
                    </p>
                </div>
            </div>";
    }
}