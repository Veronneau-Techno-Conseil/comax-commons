# Event Mailbox

The EventMailbox grain plays a huge role in our system. It holds all the messages made for a specific user in a queue. Without it, messages will end up lost.

Messages could be either user specific, group specific or to everyone on the network.
Messages will go through the Dispatch Service to be dispatched the a specific EventMailbox for the destination.
EventMailbox subscribes to MailObservers to listen for related mails.

The EventMailbox will allow users to:
- Check if he has a mail in his mailbox
- Mark the mail as read
- Delete a mail
- stream a mail

<a href="https://ibb.co/3fDn63H"><img src="https://i.ibb.co/jVcS0NF/mailbox.png" alt="mailbox" border="0"></a>

