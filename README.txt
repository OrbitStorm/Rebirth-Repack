========================
== This Archive       ==
========================
This is an official, authorized release of the UOGamers: Rebirth scripts and core source code as 
well as a complete (minus accounts) world save from September 20, 2005.  This is everything you 
need to play UOGamers: Rebirth as it was in September 2005 or to use the code to create your 
own shard with a similar ruleset.

Countless hours of programming, testing, world building, and playing time went into the content 
of this file.  It is my sincere hope that this package will bring happiness and fun to some 
nostalgic players and administrators somewhere.

========================
== Background         ==
========================
UOGamers: Rebirth was a free UO shard run by the official UOGamers/RunUO team from 2004 - 2006.  
The original idea behind Rebirth was to be as accurate as possible to Ultima Online as it existed
in late 1997 and early 1998 (as it was less than a year after it came out).  That was the era 
that most of us (at the time in 2003/2004, anyway) first fell in love with UO.  In my opinion, 
the shard was very successful in that goal.  

Like most shards, it eventually fell from popularity and many of the staff lost interest. In 
an attempt to get more players, UOGamers: Rebirth eventually closed and was replaced by 
UOGamers: Divinity which had an "updated" (to Pre-UOR) ruleset, but which was ultimately less 
authentic.

========================
== Support            ==
========================
This code is not support by anyone.  Not the original author.  Not RunUO.  Not UOGamers.  No one.
Toying with this code is probably not for the novice.  If you look at it and feel yourself 
itching to ask a question (and can't figure out the answer from the code itself or from this 
readme) then STOP! You're in over your head!  There is no one to turn to for help. Just give up.

========================
== License            ==
========================
The worldfile is property of UOGamers and should not be used as the basis for any public shard 
without express permission from UOGamers/RunUO.  Some elements of the worldfile may be the 
intellectual property of players or staff of the original shard.

The RunUO source code and most of the scripts fall under the standard RunUO software license, 
see www.runuo.com for more information.  This also applies to the data.  All code belonging to 
solely to "Zippy" is hereby released into the public domain free for use in any way without 
any restrictions.

Ultima Online belongs to Electronic Arts, Inc.  They may have intellectual property rights to 
portions of this package; I honestly don't know.

If you use this code on your own server, you should let it be known where the code originally 
came from.

========================
== The Code           ==
========================
== Scripts ==
Rebirth used the RunUO 1.0.1 core originally.  This package contains an updated version of the 
scripts with only the changes required for the scripts to run on a modern (2.1+/SVN) core which 
supports modern (7.0+) clients.  Other small changes were made to facilitate ease of use, but no 
changes/updates/bugfixes were made to the core ruleset. "Private" scripts such as the UOGamers 
Event/Tournament system have also been removed.

Most of the standard UO scripts in existence in 2004/2005 are included in the package even though 
they aren't authentic.  Just ignore them if you don't want to see them.

== Core ==
The scripts require a few small changes to the RunUO core to work properly.  The core RunUO.exe 
included in this package has been built with these changes, but the source was not included.  
The only changes required to the RunUO core can be found in "Rebirth-Core-svn728.diff".

The most interesting core changes were made to the packet code.  These changes convert any 
message sent by any script into ASCII english text.  This is because for the target era/ruleset 
Unicode font and localization did not exist in Ultima Online.  Rather than modify all scripts 
that send messages, this code converts the Unicode into ascii before sending the packets. 

========================
== The World          ==
========================
This world save was originally created on UOGamers: Rebirth on September 20, 2005 at 12:22pm.

The accounts have been removed to protect anyone still using those emails/usernames/passwords, 
however all items and characters remain exactly as they were.  The houses have been set "ageless" 
so that they won't decay as soon as the world has finished loading, so that they can be freely 
explored.

All original player characters are still in the file, but since there are no accounts they cannot 
be easily logged in to.  The following are instructions on how to manually link a player 
character to a new account:
1) Log in as an administrator and execute the following command:
[global interface where playermobile name = "The Character I am looking for"
2) This will search for all players by that name.  Use the interface window to bring the characters 
to your location.  (Do NOT go to their location.)
3) Once you have located the proper player, execute the command [get serial and target the player.
Write down the number that is given to you.
3a) If the number you are given begins with "0x", type the entire number into google followed by 
"to decimal" and use the result google gives you instead.  For example, if [get serial gave you 
"0x1234" you would search google for "0x1234 to decimal" which would yield the result "4660" 
which is the number you should use for the rest of these instructions.
4) Shut down the server and open the Saves/Accounts/Accounts.xml file
5) Find the account you are looking for.  If it does not have a "<chars>" block, add an empty one.
If you don't know how, look at other accounts or start the server back up and log into the 
account and create a new character, save the world, then shut the server down again.
6) Add a "<char>" entry like the one below, substitute the number you wrote down in step #3/#3A 
for the XXX below.
<char index="0">XXX</char>
7) Save the file and start the world again.

========================
== The Ruleset        ==
========================
This is a brief and incomplete listing of the major features of the Pre-T2A ruleset.  This is not 
a complete list, it is only what I could remember off the top of my head.

== Notoriety System ==
- Pre-T2A, Ultima Online had a "notoriety" system in which everyone was either good, or evil.  
This is similar to modern UO's concept of "Karma", but "Karma" and "Fame" were lumped together.  
One could turn "Red" ("Murderer" in modern UO) relatively easily, including by killing too many 
"Blue" NPCs.  Once could easily become "Perma-Grey" from stealing or other unsavory acts.  

== Spell System ==
-Honestly I no longer remember all of the changes to magic that happened over the years.  The 
magic system in Rebirth is as authentic as we could make it from memory and some other references.

== Bounty System ==
-Just like that which originally existed in early 1998, a bounty system in-which the Dread Lord's 
head must be removed from his corpse and given to an NPC guard for a reward which was originally 
funded by the Read's victims.  "Bounty Boards" can be set up in town to see the names and 
bounties for each person.  When a player is murdered, they are allowed to contribute gold from 
their bank toward the bounty.

== Guild System ==
-Using the old-style "gump" windows

== Chaos/Order System ==
-Precursor to Factions.  A guild may declare themselves "Chaos" or "Order".  Members may then go 
to the Lords' Castles and request a virtue shield from the castle guard.  While wearing the 
shield, players may fight any players of the opposite virtue even while in town (even if the 
guilds have not declared war).

== Skill Atrophy ==
-Before T2A there were no skill locks.  When nearing the skill cap, skills could accidentally 
drop as others were gained.  It became increasingly difficult to achieve "7x", the last 1 or 2 
points taking extremely long to gain (while having to repeatedly "ping-pong" between skills).

== Weapon Modifiers ==
-Original weapon modifiers including spell-casting weapons.

== Item Names ==
-Something I'm very proud of, all items have proper single-click names and NO context-menus. This 
includes weapons (ex: "You See: an indestructible halberd of vanquishing and mage's bane with 10 
charges")

== Talking while hidden ==
-This one was difficult because it was originally somewhat of a client bug that was fixed about  
13 years ago. But we did manage to hack around it, and you can speak while hidden.  When you do, 
your words will appear at your feet and anyone in the area can see them and know where you are, 
but you will not be unhidden.  Keep in mind this is back before party talk, so if you wanted to 
hide and wait for some blues but also talk to your homies... you get the idea.

== Communication Crystals ==
-For long distance chat, you can buy communication crystals and link them together.  Power them 
with gems, they can be used to send messages a long way.  But also cool for house security, since 
you could leave one in your house and if anyone talked inside you'd know.

== Speaking of Houses ==
-No rules, just right.  No lock downs, no secures.  Nothing in houses (except corpses) decays. If 
you lose your key, whoever finds it effectively owns your house at that point.  Also if someone 
sneaks inside they can just start grabbing stuff since it is locked down.

== Player Vendors ==
-Can be used to semi-securely store house keys

== Crafting ==
-Original UO crafting (which is what you'll find in these scripts) was very sparse. There were no 
special items or special resource gathering bonuses.  Crafting here is very basic.

== Colored Armor ==
-NPCs sell colored armor, ranging from deep black to rose colored.  There was no special Ore to 
mine, so the only way to get non-silver armor is to buy from NPCs.  They sell random colors, so 
getting a complete set can be a bit of a challenge.

== Duping ==
-The one thing we didn't add was server crashes and gold duping. Ha.

== Skill Gain ==
-This formula is all our own creation, we couldn't figure out the formulas in UODemo.  It is 
fairly hard, and it works on a more official UO-like "Curve" where making the most difficult 
thing you can is *not* always the best way to gain skill (instead, make the thing that you can 
succeed at ~60% of the time...)
-Skill Gain rate is also partially impacted by how much that skill is being used globally 
compared to other skills.  So odd, rare skills are easier to gain.  (Source: internet archive)
-Does food effect skill gain rate or success chance? You'd have to look in the code to see...

== Christmas Presents ==
-The original UO Christmas presents from 1997 are all there.  Does coal do anything?  You'd have 
to look in the code to find out....

== Purple Llama Vortex? ==
Yeah, we've got that.

== Resurrect Now Option ("Fred vs. Bob" Wars) ==
-Back in the early days there was an option to resurrect instantly after you died, at the cost of 
some stats and skills.  While you'd of course never do this with a good char, sometimes it was 
fun to make a new char and just mess around with newbie skills and equipment.
-Back on the official servers, when one went down (happened a lot back then) people from that 
server would pick a different and make a char named "Fred" or "Bob" and all meet up at a random 
place and have huge fights, or grief the regular players of that shard until their came back up. 
It was great fun; usually hilarious.

== Horses (and your old friend, Lag) ==
-In 1997/1998, most people were on 28.8 or 33.6 modems.  I think ADSL was just starting to get 
popular.  Also hardware was not nearly as powerful as it is now, and so "Lag" (server<->client 
latency) was a serious issue.  Normal ping times were in the 200-400ms range.
-For that reason, running full speed on a horse did not give most people a serious advantage 
over running on foot.  Also there were no ethereal mounts, and so horse riding was somewhat rare. 
In order to demotivate people from using horses on Rebirth, we implemented a "fake lag" feature 
which causes horses to lag when they make turns. They can run fast speed (for the most part) when 
going straight for long distances, but in combat they are generally more trouble than they are 
worth.

========================
== The Sources        ==
========================
The implementation of this ruleset was driven by several sources, the foremost being my memory 
and the memory of Rebirth's players.

Also, UOdemo... see below.

Additionally, in 2005 there was a website which had the Origin "publish notes" for every patch 
ever made to Ultima Online.  Some of these provided very helpful explanations of how certain 
systems worked.  Unfortunately, that site has been lost to time.. I don't have the link anymore 
and I think it is probably down now anyway.

The "Wayback Machine" provided useful access to ancient discussion forum posts, which 
supplemented memory and patch logs on occasion.  These posts, though, were often just rumors and 
guesses from players of the time, only slightly more reliable than our own memories.

========================
== UOdemo             ==
========================
An oft-forgotten application included on the original "Ultima Online: The Second Age" (T2A) 
CD-Rom was a program called "UODemo" which allowed limited single player UO without an internet 
connection.  The CD contained a working version of the Ultima Online official server.  The UO 
emulator community (definitely I cannot take any credit for any work that went into this) was 
able to figure out the compression/encryption on the uodemo data files.  This data included loot 
tables, equipment and NPC statistics, and much much more.

This data is the "primary source" for most of the numbers in the Rebirth Scripts (spells, damage,
weapons, combat formulas, skill formulas, etc).  

Unfortunately I no longer have any of the UODemo files or related tools/programs.  They may still
be lurking out on the internet somewhere, or they may be lost to time now.  Anyone with an 
original T2A disc, a little programming skill, and a lot of free time might be able to recreate 
some of the tools if they can't be found.  T2A discs can still be found for sale with a little 
bit of digging.
