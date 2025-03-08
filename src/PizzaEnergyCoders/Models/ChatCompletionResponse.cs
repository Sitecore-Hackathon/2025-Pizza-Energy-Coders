using System.Collections.Generic;

namespace PizzaEnergyCoders.Models
{
    /// <summary>
    /// Model for Open AI Response
    /// </summary>
    public class ChatCompletionResponse
    {
        public string Id { get; set; }
        public string Object { get; set; }
        public long Created { get; set; }
        public string Model { get; set; }
        public List<Choice> Choices { get; set; }
        public Usage Usage { get; set; }
        public string ServiceTier { get; set; }
        public string SystemFingerprint { get; set; }
        public string StatusCode { get; set; }
    }
    /// <summary>
    /// Model to manage the message response
    /// </summary>
    public class Choice
    {
        public int Index { get; set; }
        public Message Message { get; set; }
        public object Logprobs { get; set; }
        public string FinishReason { get; set; }
    }
    /// <summary>
    /// Model to manage the message 
    /// </summary>
    public class Message
    {
        public string Role { get; set; }
        public string Content { get; set; }
        public object Refusal { get; set; }
    }
    /// <summary>
    /// Model to set the config for Open AI
    /// </summary>
    public class Usage
    {
        public int PromptTokens { get; set; }
        public int CompletionTokens { get; set; }
        public int TotalTokens { get; set; }
        public PromptTokensDetails PromptTokensDetails { get; set; }
        public CompletionTokensDetails CompletionTokensDetails { get; set; }
    }
    /// <summary>
    /// Model for tokens
    /// </summary>
    public class PromptTokensDetails
    {
        public int CachedTokens { get; set; }
        public int AudioTokens { get; set; }
    }

    /// <summary>
    /// Model for Tokens Completion
    /// </summary>
    public class CompletionTokensDetails
    {
        public int ReasoningTokens { get; set; }
        public int AudioTokens { get; set; }
        public int AcceptedPredictionTokens { get; set; }
        public int RejectedPredictionTokens { get; set; }
    }
}