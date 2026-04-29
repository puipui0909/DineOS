import * as signalR from "@microsoft/signalr";

let connection = null;

export const startOrderRealtime = async (onReceive) => {
  connection = new signalR.HubConnectionBuilder()
    .withUrl("http://192.168.1.161:5010/orderHub")
    .withAutomaticReconnect()
    .build();

  connection.on("ReceiveOrderUpdate", (order) => {
    onReceive(order);
  });

  await connection.start();
};

export const stopOrderRealtime = async () => {
  if (connection) {
    await connection.stop();
  }
};