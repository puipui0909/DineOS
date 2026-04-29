import { useEffect, useState } from 'react';
import { Box, IconButton, Tooltip } from '@mui/material';
import ChatIcon from '@mui/icons-material/Chat';

export default function ChatToggle({ onClick }) {
  const [openTooltip, setOpenTooltip] = useState(true);

  useEffect(() => {
    // tự tắt sau 12 giây (bạn có thể chỉnh 10–15s)
    const timer = setTimeout(() => {
      setOpenTooltip(false);
    }, 12000);

    return () => clearTimeout(timer);
  }, []);

  return (
    <Box
      sx={{
        position: 'fixed',
        bottom: 20,
        left: 10,
        zIndex: 1000
      }}
    >
      <Tooltip
        open={openTooltip}
        title="Bạn chưa biết nên gọi món nào? Hãy để tôi gợi ý cho bạn!"
        placement="right"
        arrow
      >
        <IconButton
          color="primary"
          onClick={() => {
            setOpenTooltip(false); // click là tắt luôn tooltip
            onClick();
          }}
          sx={{
            backgroundColor: '#fff',
            boxShadow: 3
          }}
        >
          <ChatIcon />
        </IconButton>
      </Tooltip>
    </Box>
  );
}