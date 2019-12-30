# cachy

Usernames, passwords, account ids, secret questions, memorable places, pin codes, bla bla and bla.  It seems that our mental capacity to remember small bits of vital information is being exponentially tested as we progress (technologically) as a species.  I daren't imagine how many different pieces of information the average human will be required to recall in order to maintain their electronic identity in 20 years time, or maybe they won't, who knows?

Many people have claimed to be killing off username and password combinations, I for one have worked with such companies, and I truly believe it will happen one day, but I see no evidence of this becoming a reality in the very near future.  So it seems for now, they are here to stay, are you doing all you can to keep that information secure?  If not you could be setting yourself up for a fall.

## Not Using Secure Password Management

Before we go into detail here, I would like to be clear that by "Secure Password Management", I mean that the credentials are (at a minimum) stored in an encrypted state; at rest, using relevant (up-to-date) cryptographic standards.

Here are a few things that I have personally known people to do,

* Store credentials in a Microsoft Office Word Document or Excel spreadsheet (with our without a password)
* Write credentials on a physical notepad / diary or random scraps of paper haphazardly stored within a desk
* Store credentials in a note taking application or service

All of these methods are bad, some less than others, and should be avoided. Let's quickly go over the reasons why.

#### Why you shouldn't store credentials in a Microsoft Office Word Document or Excel spreadsheet (with our without a password)

We mention Microsoft Office products here as they are the industry standard, but the same principals apply to any Office suit.  Try and use applications for what they are designed for, and Office software was **not** designed for storing credentials.

> *Before Office 2007, the encryption techniques used are so dated that the passwords can be cracked using brute force with relative ease.* New versions of Office do use more modern techniques and algorithms, such as *SHA-512* password hashing with a *salt*.  This protects the password against dictionary and brute force attacks. But still doesn't make it suitable for storing credentials.

From a non-security related perspective,

* Excel isn't free so it's a rather expensive way to store your credentials.
* Not suitable for both mobile / desktop use.  Quickly opening an encrypted spreadsheet on your mobile device, finding / adding credentials is a very clunky experience.
* Difficult to manage.  Managing a spreadsheet of credentials isn't easy to manage, unless you're a real wizard with Excel, it's unlikely that they are going to be nicely organised and easily searchable.
* It's not designed for storing credentials.

#### Why you shouldn't store credentials on a physical notepad / diary or random scraps of paper haphazardly stored within a desk

It's pretty obvious why this is a bad idea, but just incase you're not getting it, you should bear in mind that your computing device isn't the only thing you should keep secured. Performing attacks of a cryptographic nature can be extremely complex and require technical computing skills that not everyone has.  Taking a peek in your notepad while you are in the toilet, requires absoluely no skill at all.

With that said, you should keep your credentials protected against as many different attacks as possible, simple or complex.  It's all very well using a password management system to encrypt your passwords using the most advanced cryptographic algorithms currently available, but if you protect them using "password123" as your master passphrase, you're not utilising those benefits.

#### Why you shouldn't store credentials in a note taking application or service

These services are primarily designed for taking notes and helping to keep yourself organised. And while some do a very good job of this, protecting your sensitive information may not be their top priority.

Please research **any** service that you use, even if it's just for taking notes. There will usually be an article around the discussion of its security, and if you're still not sure, maybe just avoid it.

One thing you should **never** do is create a document that only has certain parts encrypted, for example,

```
Shopping List

* Carrots
* Shampoo
* Toilet Paper

Amazon.com username = alice@domain.com / password = xxxxxxxx
```

In the above text, I have shown an example of using a note taking service and encrypting only a single part of one of my notes, the amazon.com password.  Here's the problem, this note provides enough META data to know exactly what's in it, and what's being "protected", it's effectively shouting out "HACK ME!".  Securely encrypted data, does not leak *any* META data.

> META data is small pieces of information that might not seem immediately sensitive but can leak enough information in order to help leverage an attack on the encrypted data itself.

### Is it safe to store any of my data unencrypted?

Ultimately, the answer to this question, is a resounding **no**. Although the practicalities of encrypting *everything* will make it less convenient to access information quickly.  Security is a compromise of convenience, you surrender some convenience (the more the better) and in return your data is less likely to be compromised.  As technology advances, those inconveniences will become smaller, and will yield more return in security, but that won't happen over night, so you need to start taking the necessary steps yourself, and also start optimising the inconvenience that you are dealing with in order to maximise the return on your securty.

For example, taking quick notes with a pen and paper, can be replaced by taking digital notes, using something like a Galaxy Note smartphone where you can write notes that are stored on a secured device, rather than in plain view on your desk.

> Although we are not trying to scare you into being paranoid about everything, there is a healthy level of paranoia that you should try to be aware of. After all, it's your data that's at risk, you wouldn't leave your front door unlocked and then tell everyone on Facebook about it, so don't effectively do the same with your digital life by not caring about your credentials.

### Simple Security Tips

* Try to treat as much of your data as possible; as sensitive.  Although you might not think that a list of your favourite music is sensitive, it might yield enough information to an attacker in order to answer a "secret question" for a website that you use, and gain some level of access.  From that point they may then get into one of your other accounts, and so on.
* Use software for what it was designed for. For example, don't use a word processor to store your credentials, in the same way that you wouldn't use a credential manager to write a CV. Not only does this provide you with a less than optimal experience for your time, but it also puts your data at risk.
* Never re-use the same password.
* Never use passwords that can be guessed by knowing things about you. For example, if your wifes name is "Kaylee" and her year of birth is "1984", "Kaylee1984" would be one of the first passwords that a hacker may attempt to use. Preferably, passwords shouldn't contain **any** information about you.
* Where possible use a password that has been generated securely, rather than one that you have created in your head.  Some credential managers come with password generators built-in, use them!  If you can't recall your password easily, don't worry, let the credential manager take care of remembering it for you, that's what they are for!
* Change your passwords regularly. Don't keep the same password for 10 years, try to change it ever 6 to 12 months.

## Introducing cachy, The Cross-Platform Password Protection System

We would like to take a moment to introduce you to cachy, created by devoctomy (that's us) to protect your credentials, and help provide an adequate balance of convenience and security. If you're not using a credential manager at current, or using one of the methods that we have advised against, or just looking for a more cost-effective solution, then please consider cachy.

##

**As of 30th December 2019.  All source code for the cachy system, including the Azure Function app used for handling OAuth connections to the app has been made open-source.**
