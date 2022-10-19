import { HttpClient } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Message } from './_models/message';
import * as SignalR from '@microsoft/signalr';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit, OnDestroy {
  backendUrl = 'http://localhost:5500/messages/';
  messagingServerUrl = 'http://localhost:5292/messageHub';

  hubConnectionBuilder!: SignalR.HubConnectionBuilder;
  hubConnection!: SignalR.HubConnection;

  chatText!: string;
  messages: Message[] = [];

  constructor(private httpClient: HttpClient) {}

  ngOnDestroy(): void {
    if (this.hubConnection != null)
      this.hubConnection.stop().then(() => {
        console.log('SignalR Disconnected.');
      });
  }

  ngOnInit(): void {
    this.connectToSignalRHub();
    this.getMessages();
  }

  private getMessages() {
    this.httpClient.get<Message[]>(this.backendUrl).subscribe({
      next: (messages) => {
        messages.sort((a, b) => {
          if (a.sentAt > b.sentAt) return 1;
          else if (a.sentAt < b.sentAt) return -1;
          else return 0;
        });
        this.messages = messages;
      },
      error: (err) => {
        let error = JSON.stringify(err);
        let errorJson = JSON.parse(error);
        console.log('error occurred on initial server connection', errorJson);
      },
      complete: () => {},
    });
  }

  private async connectToSignalRHub() {
    this.hubConnectionBuilder = new SignalR.HubConnectionBuilder();
    this.hubConnection = this.hubConnectionBuilder
      .withUrl(this.messagingServerUrl, {
        skipNegotiation: true,
        transport: SignalR.HttpTransportType.WebSockets,
      })
      .withAutomaticReconnect([4, 10, 20, 40])
      .build();

    this.hubConnection.on('1:NewChat', (messageId: string) => {
      this.getMessages();
    });

    this.hubConnection.on('DeleteChats', () => {
      this.messages = [];
    })

    await this.start();
  }

  private async start() {
    try {
      await this.hubConnection
        .start()
        .then(() => console.log('connected to signla R'));
    } catch (err) {
      console.log(err);
    }
  }

  sendChat() {
    let newChat = this.chatText;
    this.chatText = '';

    this.sendMessageToApi(newChat);
  }

  private sendMessageToApi(message: string) {
    this.httpClient
      .post<Message>(this.backendUrl, { senderId: 1, body: message })
      .subscribe({
        next: (newMessage) => {
          this.messages.push(newMessage);
          this.NotifySignalRForNewMessage(newMessage.id);
        },
        error: (err) => {
          console.log(err);
        },
      });
  }

  deleteAllMessages() {
    this.httpClient.delete(this.backendUrl).subscribe({
      next: () => {
        this.messages = [];
        this.hubConnection.send('NotifyDelete');
      },
      error: () => {
        alert('error occurred');
      },
    });
  }

  private NotifySignalRForNewMessage(messageId: string) {
    this.hubConnection.send('NotifyNewMessage', 2, messageId);
  }
}
