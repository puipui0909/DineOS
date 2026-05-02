// orderRealtime.js
import * as signalR from "@microsoft/signalr";

export let connection = null;

// Chuyển thành Set để tránh trùng lặp hàm
const newOrderListeners = new Set();
const updateOrderListeners = new Set();

export const startOrderRealtime = async (onNewOrder, onUpdateOrder) => {
  // Thêm callback vào danh sách thay vì ghi đè
  if (onNewOrder) newOrderListeners.add(onNewOrder);
  if (onUpdateOrder) updateOrderListeners.add(onUpdateOrder);

  if (connection && (connection.state === "Connected" || connection.state === "Connecting")) {
    return;
  }

  if (!connection) {
    connection = new signalR.HubConnectionBuilder()
      .withUrl("https://your-api.onrender.com/orderHub", {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect([0, 2000, 5000, 10000])
      .build();

    connection.on("OrderCreated", (order) => {
      newOrderListeners.forEach(callback => callback(order));
    });

    connection.on("OrderUpdated", (order) => {
      updateOrderListeners.forEach(callback => callback(order));
    });
    
    // Thêm log để theo dõi số lượng listener cho đồ án
    console.log(`📡 SignalR Listeners: New(${newOrderListeners.size}) Update(${updateOrderListeners.size})`);
  }

  try {
    await connection.start();
    console.log("✅ SignalR Connected");
  } catch (err) {
    console.error("❌ SignalR Start Error:", err);
  }
};

// Quan trọng: Phải có hàm để gỡ callback khi component unmount
export const stopOrderRealtime = (onNewOrder, onUpdateOrder) => {
  if (onNewOrder) newOrderListeners.delete(onNewOrder);
  if (onUpdateOrder) updateOrderListeners.delete(onUpdateOrder);
  
  if (newOrderListeners.size === 0 && updateOrderListeners.size === 0 && connection) {
     // Chỉ đóng kết nối nếu không còn ai nghe nữa (tùy bạn quyết định)
     // connection.stop(); 
  }
};