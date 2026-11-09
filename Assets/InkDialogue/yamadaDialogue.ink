-> 00_Intro

=== 00_Intro ===
Hey there. I guess you're the new Carrier.

There's some cores laying around that need to be collected and deposited at central.
-> 00_Prompt

=== 00_Prompt ===
I’m not exactly built for that so - if you would be so kind to serve your purpose I think we’d all appreciate it.
+ [Okay]
    ...okay
    -> 01_Start
+ [Why the attitude]
    Try being a desk droid for a century, who knows, maybe you’ll end up more likeable than me.
    -> 00_Prompt
+ [Bye]
    Okay? Where would you even go?
    -> END


=== 01_Start ===
You gonna take the job?

+ [Accept]
    Good luck out there, don't break down somewhere in the wilderness. Carriers don't appear much nowadays. Would suck to have to find a replacement in this economy.
    // ~ StartDelivery()
    -> END

+ [Not now]
    Not like I got better things to do. Come back when you're ready I guess.
    -> END