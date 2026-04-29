import { useState } from 'react';
import {
  Box,
  TextField,
  Button,
  Typography,
  Card,
  CardContent,
  Chip
} from '@mui/material';
import { aiService } from '../../../api/aiService';

export default function ChatBox({ onClose }) {
  const [messages, setMessages] = useState([]);
  const [input, setInput] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSend = async () => {
    if (!input.trim()) return;

    // user message
    const userMsg = { type: 'user', text: input };
    setMessages(prev => [...prev, userMsg]);

    setLoading(true);

    try {
      const data = await aiService.suggest(input);

      const botMsg = {
        type: 'bot',
        suggestions: data
      };

      setMessages(prev => [...prev, botMsg]);
    } catch (err) {
      setMessages(prev => [
        ...prev,
        { type: 'bot', error: true }
      ]);
    }

    setInput('');
    setLoading(false);
  };

  return (
    <Box
      sx={{
        position: 'fixed',
        bottom: 80,
        left: 10,
        right: 10,
        width: {
          xs: 'auto',
          sm: 350
        },
        height: {
          xs: '70vh',
          sm: 500
        },
        backgroundColor: '#fff',
        border: '1px solid #ccc',
        borderRadius: 2,
        boxShadow: 6,  
        display: 'flex',
        flexDirection: 'column',
        p: 1,
        overflow: 'hidden'   ,
        zIndex: 1000 
      }}
    >
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={1}>
        <Typography fontWeight="bold">AI Gợi ý</Typography>

        <Button size="small" onClick={onClose}>
          ✕
        </Button>
      </Box>
      {/* Chat content */}
      <Box sx={{ flex: 1, overflowY: 'auto', mb: 1 }}>
        {messages.map((msg, index) => (
          <Box key={index} mb={2}>
            
            {/* User */}
            {msg.type === 'user' && (
              <Box textAlign="right">
                <Chip label={msg.text} color="primary" />
              </Box>
            )}

            {/* Bot */}
            {msg.type === 'bot' && (
              <Box textAlign="left">
                
                {msg.error && (
                  <Typography>Lỗi rồi 😢</Typography>
                )}

                {msg.suggestions && (
                  <>
                    <Typography mb={1}>
                      🤖 Gợi ý cho bạn:
                    </Typography>

                    {msg.suggestions.map(item => (
                      <Card key={item.id} sx={{ mb: 1 }}>
                        <CardContent>
                          <Typography fontWeight="bold">
                            {item.name}
                          </Typography>

                          <Typography variant="body2">
                            {item.description}
                          </Typography>

                          <Typography
                            variant="caption"
                            color="text.secondary"
                          >
                            👉 {item.reason}
                          </Typography>
                        </CardContent>
                      </Card>
                    ))}
                  </>
                )}
              </Box>
            )}
          </Box>
        ))}

        {loading && (
          <Typography>Đang gợi ý...</Typography>
        )}
      </Box>

      {/* Input */}
      <Box display="flex" gap={1}>
        <TextField
          fullWidth
          size="small"
          value={input}
          onChange={(e) => setInput(e.target.value)}
          onKeyDown={(e) =>
            e.key === 'Enter' && handleSend()
          }
        />

        <Button
          variant="contained"
          onClick={handleSend}
        >
          Gửi
        </Button>
      </Box>
    </Box>
  );
}