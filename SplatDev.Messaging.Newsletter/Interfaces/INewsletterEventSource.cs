namespace SplatDev.Messaging.Newsletter.Interfaces
{
    using SplatDev.Messaging.Newsletter.Events;

    public interface INewsletterEventSource
    {
        event EventHandler<NewsletterEventArgs>? Subscribed;

        event EventHandler<NewsletterEventArgs>? Unsubscribed;

        event EventHandler<NewsletterEventArgs>? Bounced;

        event EventHandler<NewsletterEventArgs>? Complained;

        event EventHandler<NewsletterEventArgs>? Opened;

        event EventHandler<NewsletterEventArgs>? Clicked;

        event EventHandler<NewsletterEventArgs>? CampaignSent;
    }
}
