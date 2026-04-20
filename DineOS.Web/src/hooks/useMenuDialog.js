import { useState } from 'react';

export const useMenuDialog = () => {
  const [openForm, setOpenForm] = useState(false);
  const [openDelete, setOpenDelete] = useState(false);

  // CREATE
  const openCreate = () => {
    setOpenForm(true);
  };

  // EDIT
  const openEdit = (setEditingItem, item) => {
    setEditingItem(item);
    setOpenForm(true);
  };

  // CLOSE FORM
  const closeForm = (setEditingItem) => {
    setEditingItem(null);
    setOpenForm(false);
  };

  // DELETE
  const openDeleteDialog = (setDeleteId, id) => {
    setDeleteId(id);
    setOpenDelete(true);
  };

  const closeDeleteDialog = () => {
    setOpenDelete(false);
  };

  return {
    openForm,
    openDelete,

    openCreate,
    openEdit,
    closeForm,

    openDeleteDialog,
    closeDeleteDialog
  };
};