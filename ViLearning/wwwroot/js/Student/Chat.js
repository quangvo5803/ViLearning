var currentUserId = document.getElementById("hiddenUserId").value;
var receiverUserId = document.getElementById("hiddenReceiverId").value;
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

// Disable the send button until connection is established
// document.getElementById(sendButton).disabled = true;

connection.on("ReceiveMessage", function (userId, message, sendAt) {
    var msgDiv;

    if (userId === currentUserId) {
        // Tạo các div cho tin nhắn của người gửi
        msgDiv = document.createElement("div");
        msgDiv.classList.add("outgoing_msg");

        var sentMsgDiv = document.createElement("div");
        sentMsgDiv.classList.add("sent_msg");

        var pElement = document.createElement("p");
        pElement.textContent = message;

        var spanElement = document.createElement("span");
        spanElement.classList.add("time_date");
        spanElement.textContent = sendAt;

        // Append các phần tử vào đúng vị trí
        sentMsgDiv.appendChild(pElement);
        sentMsgDiv.appendChild(spanElement);

        msgDiv.appendChild(sentMsgDiv);
    } else {
        // Tạo các div cho tin nhắn của người nhận
        msgDiv = document.createElement("div");
        msgDiv.classList.add("incoming_msg");

        var receivedMsgDiv = document.createElement("div");
        receivedMsgDiv.classList.add("received_msg");

        var receivedWithdMsgDiv = document.createElement("div");
        receivedWithdMsgDiv.classList.add("received_withd_msg");

        var pElement = document.createElement("p");
        pElement.textContent = message;

        var spanElement = document.createElement("span");
        spanElement.classList.add("time_date");
        spanElement.textContent = sendAt;

        // Append các phần tử vào đúng vị trí
        receivedWithdMsgDiv.appendChild(pElement);
        receivedWithdMsgDiv.appendChild(spanElement);

        receivedMsgDiv.appendChild(receivedWithdMsgDiv);
        msgDiv.appendChild(receivedMsgDiv);
    }

    // Append msgDiv vào messagesList
    var messageList = document.getElementById("messagesList");
    messageList.appendChild(msgDiv);

    // Tự động cuộn xuống cuối
    messageList.scrollTop = messageList.scrollHeight;
});


connection.start().then(function () {
    document.getElementById("msg_send_btn").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("msg_send_btn").addEventListener("click", function (event) {
    var messageInput = document.getElementById("messageInput");
    var message = messageInput.value.trim(); // Loại bỏ khoảng trắng ở đầu và cuối chuỗi
    if (message === "") {
        // Nếu chuỗi rỗng, không gửi tin nhắn
        return;
    }

    /*connection.invoke("SendMessage", currentUserId, message).then(function () {
        // Xóa nội dung của input sau khi gửi tin nhắn thành công
        messageInput.value = "";
    }).catch(function (err) {
        console.error("Error sending message: ", err.toString());
    });*/

    connection.invoke("SendMessageTo",receiverUserId, currentUserId, message).then(function () {
        // Xóa nội dung của input sau khi gửi tin nhắn thành công
        messageInput.value = "";
    }).catch(function (err) {
        console.error("Error sending message: ", err.toString());
    });
    event.preventDefault();
});