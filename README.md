# PERMISSION SERVER

DotNet Nuget package providing password-less authentication with auto-expiring tokens (in an in-memory store; no database required) and confirmation emails.

*Being licensed under the [AGPL](./LICENSE.txt) you are free to use Permission Server in any project whether open source, free, or commercial. For further clarity see here.*

**[Status:](./CHANGELOG.md)** Work in progress

*Copyright 2023 K Cartlidge.*

## Contents

- [Initial features](#initial-features)
- [Planned extra features](#planned-extra-features)
- [Overview](#overview)
  - [How does it work?](#how-does-it-work)
  - [For personal sites](#for-personal-sites)
  - [For larger sites](#for-larger-sites)
- [What is the downside of storing tokens in memory?](#what-is-the-downside-of-storing-tokens-in-memory)
- [What if the user's email is compromised?](#what-if-the-users-email-is-compromised)
- [How do I use Permission Server in my project?](#how-do-i-use-permission-server-in-my-project)
- [Licensing details](#licensing-details)

## Initial features

- Email address validation
- Configurable-lifetime auto-expiring tokens
- Minimal supporting code required
- No database needed

## Planned extra features

- Optional database token storage
- Optional managing of accounts and access

## Overview

The hardest details to steal are those that are never stored in the first place.

With Permission Server you get the security of confirmed email addresses for both account creation and login without the need to store passwords, security questions, or similar. Reduce your risk and for many apps/sites your overheads too due to the ability to skip a database.

### How does it work?

- Your app/site asks for an email address (only)
- Permission Server generates a token and emails it to the user
  - The user gets the token (their email provider is indirectly applying security *for* you)
- Your app/site has a confirmation screen/page asking for the email address and token
  - Having the token from the email proves email address ownership
- Permission Server validates the token is a match and has not expired
- Your app/site can now either create an account or sign into one

### For personal sites

You may be writing your own blog or content site and want editing features.
To allow this you need some kind of authentication to enable access to an admin area.

As you don't need passwords you're free to store a list of admin email addresses in your appsettings file (for example).  Your login screen takes an email address and checks it against that allow-list.  If it's supported it emails a token.  By accessing that token in your email you prove who you are and can enter the admin area.

### For larger sites

You'll probably have a database already, but there's no need to risk data loss by adding any password or recovery details into it; your database is unchanged.

When a user confirms their email address via Permission Server you either sign them into their account (which exists in your database with a matching email address) or you take them through account creation based on that email address knowing that they have proven control of it.

## What is the downside of storing tokens in memory?

There *is* a downside. How important it *really* is to you depends upon your business, your site reliability, and your release frequency.

Before saying more let me first remind you that not having a database:

- Reduces server requirements
- Provides simpler deployments
- Increases performance
- Eliminates database management
- Removes a data source for hackers

The downside is that active tokens are lost whenever your app/site dies, is restarted, or has a new release. This means those tokens will not work.

However there are mitigating factors:

- A stable app/site should rarely be down
- If it *is* down your users can't use it anyway so your tokens are irrelevant
- If it's down for more than your token lifetime (which is measured in minutes) then those tokens would have expired anyway
- You probably advised of a maintenance window
- Existing sessions will be maintained if you are using session cookies or similar; only token confirmations are affected
- Users can simply request another token (try again)

For the vast majority of cases these mitigations mean that the benefits of no database outweigh the limitation.

There is also the secondary downside of tokens consuming app/site memory. However volumes are usually very low and all used or expired tokens are automatically removed every time a new token is generated. Token growth is therefore limited to the window of your token lifetime.

Database support is a planned future feature if you still need it so watch this space.

## What if the user's email is compromised?

We're relying on user access to a token in an email to prove email ownership.  If their email is compromised that opens up access to the token and therefore any other apps/sites protected by Permission Server.

However *this is no more risky than a normal password-based system*.

Why?  A password-based system needs a forgotten password or password reset feature.  This works via their emails, which means if their email account has been compromised they are equally at risk.

The only protection is another factor (eg SMS or TOTP codes) which you are free to add to a password-less system as well as a password-based one and requires the same amount of effort regardless.

## How do I use Permission Server in my project?

As the codebase progresses this section will be updated.

## Licensing details

*Being licensed under the [AGPL](./LICENSE.txt) you are free to use Permission Server in any project whether open source, free, or commercial. For further clarity see here.*

There is no requirement to share your own project's code.  You need to share any changes you make to Permission Server itself, and the AGPL won't let you run Permission Server as a stand-alone cloud-based tool. Again, though, there's no restrictions at all on what you do with your own stuff that you add Permission Server into; that's none of my business. If in doubt read the license, but generally unless you are changing Permission Server or wanting to run Permission Server as a stand-alone online service there's nothing to worry about.

---

*Please do not open the `.editorconfig` file in the Visual Studio UI as there is a likelihood it will become filled with VS-specific entries.*
