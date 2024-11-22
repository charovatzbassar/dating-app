import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
} from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { User } from '../_models/user.model';

@Injectable({
  providedIn: 'root',
})
export class PresenceService {
  hubUrl = environment.hubsUrl;
  private hubConnection?: HubConnection;
  private toastr = inject(ToastrService);
  onlineUsers = signal<string[]>([]);

  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'presence', {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.start().catch((e) => {
      console.error(e);
    });

    this.hubConnection.on('UserIsOnline', (username) => {
      this.toastr.info(username + ' is online.');
    });

    this.hubConnection.on('UserIsOffline', (username) => {
      this.toastr.warning(username + ' is offline.');
    });

    this.hubConnection.on('GetOnlineUsers', (currentUsers) => {
      this.onlineUsers.set(currentUsers);
    });
  }

  stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection.stop().catch((e) => console.error(e));
    }
  }
}
