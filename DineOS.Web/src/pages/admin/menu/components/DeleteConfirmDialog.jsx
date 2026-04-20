import { Dialog, DialogTitle, DialogContent, DialogActions, Button } from '@mui/material';

const DeleteConfirmDialog = ({ open, onClose, onConfirm, item }) => {
  return (
    <Dialog open={open} onClose={onClose}>
      <DialogTitle>Xác nhận xoá</DialogTitle>
      <DialogContent>
        Bạn có chắc muốn xoá "{item?.name}" không?
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Huỷ</Button>
        <Button color="error" onClick={onConfirm}>
          Xoá
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default DeleteConfirmDialog;