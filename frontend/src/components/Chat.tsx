import React, { useState, FormEvent, useEffect, useRef } from 'react';
import appInsights from './telemetry';
import './chat.css'; // Import the CSS file

interface Message {
  id: number;
  sender: 'user' | 'bot';
  text: string;
}

function Chat() {
  const [messages, setMessages] = useState<Message[]>([
    { id: 1, sender: 'bot', text: 'Hello! How can I assist you today?' },
  ]);
  const [input, setInput] = useState('');
  const [isLoading, setIsLoading] = useState<boolean>(false); // State to track loading
  const messagesEndRef = useRef<HTMLDivElement>(null); // Reference to the end of messages

  // Function to scroll to the bottom of messages
  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' });
  };

  // Track Page View on Component Mount
  useEffect(() => {
    appInsights.trackPageView({ name: 'Chat Page' });
  }, []);

  // Scroll to bottom whenever messages change
  useEffect(() => {
    scrollToBottom();
  }, [messages]);

  // Function to handle sending messages
  const sendMessage = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    if (!input.trim() || isLoading) return; // Prevent sending if already loading

    // Track Custom Event for Sending Message
    appInsights.trackEvent({ name: 'send_message', properties: { messageContent: input } });

    const newMessage: Message = {
      id: messages.length + 1,
      sender: 'user',
      text: input,
    };
    setMessages([...messages, newMessage]);
    setIsLoading(true); // Start loading

    const apiEndpoint = process.env.REACT_APP_API_BASE_URL;

    if (!apiEndpoint) {
      console.error('API endpoint is not defined. Please set REACT_APP_API_BASE_URL in your .env file.');
      appInsights.trackException({ exception: new Error('API endpoint is not defined') });
      setIsLoading(false); // End loading
      return;
    }

    try {
      const response = await fetch(apiEndpoint, {
        method: 'POST',
        headers: { 
          'Content-Type': 'application/json'
        },
        body: '"' + input + '"',
      });

      if (!response.ok) {
        throw new Error('Network response was not ok');
      }

      const data = await response.json();
      
      // Handle the response array
      const botMessages = data.map((item: any, index: number) => ({
        id: messages.length + 2 + index,
        sender: item.authorName || 'bot', // Use authorName from response
        text: item.content, // Access 'content' from the response (lowercase 'c')
      }));

      setMessages([...messages, newMessage, ...botMessages]);
      setInput('');
      
      // Track Success Event
      appInsights.trackEvent({ name: 'send_message_success' });
    } catch (error: any) {
      console.error(error);
      // Track Exception
      appInsights.trackException({ exception: error, properties: { message: error.message } });
    } finally {
      setIsLoading(false); // End loading
    }
  };

  // Function to handle feedback
  const handleFeedback = (messageId: number, feedback: 'thumbs_up' | 'thumbs_down') => {
    // Track Feedback Event
    appInsights.trackEvent({ 
      name: 'feedback', 
      properties: { 
        messageId: messageId.toString(), 
        feedback: feedback 
      } 
    });

    console.log(`Feedback received for message ${messageId}: ${feedback}`);
    // Optionally, update UI to reflect feedback submission
  };

  return (
    <div className="chat-container">
      <div className="chat-header">Chat Construction Project Management Bot</div>
      <div className="messages">
        {messages.map((msg) => (
          <div key={msg.id} className={`message ${msg.sender}`}>
            <span className="sender">{msg.sender === 'user' ? 'You' : 'Bot'}</span>
            <span>{msg.text}</span>
            {msg.sender === 'bot' && (
              <div className="feedback-buttons">
                <button 
                  className="feedback-button thumbs-up" 
                  onClick={() => handleFeedback(msg.id, 'thumbs_up')}
                  aria-label="Thumbs Up"
                >
                  👍
                </button>
                <button 
                  className="feedback-button thumbs-down" 
                  onClick={() => handleFeedback(msg.id, 'thumbs_down')}
                  aria-label="Thumbs Down"
                >
                  👎
                </button>
              </div>
            )}
          </div>
        ))}
        <div ref={messagesEndRef} /> {/* Dummy div to scroll into view */}
        {isLoading && (
          <div className="spinner-container">
            <div className="spinner"></div>
          </div>
        )}
      </div>
      <form className="input-form" onSubmit={sendMessage}>
        <input 
          value={input} 
          onChange={(e) => setInput(e.target.value)} 
          placeholder="Type your message here" 
          aria-label="Chat input"
          disabled={isLoading} // Disable input while loading
        />
        <button 
          type="submit" 
          onClick={() => appInsights.trackEvent({ name: 'SendButtonClick', properties: { buttonName: 'Send Message' } })}
          disabled={isLoading} // Disable button while loading
        >
          {isLoading ? 'Sending...' : 'Send'}
        </button>
      </form>
    </div>
  );
}

export default Chat;