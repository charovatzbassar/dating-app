import { inject, Injectable, signal } from '@angular/core';
import { environment } from '../../environments/environment';
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
} from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { User } from '../_models/user.model';
import { take } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class PresenceService {
  hubUrl = environment.hubsUrl;
  private hubConnection?: HubConnection;
  private toastr = inject(ToastrService);
  private router = inject(Router);
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
      this.onlineUsers.update((users) => [...users, username]);
    });

    this.hubConnection.on('UserIsOffline', (username) => {
      this.onlineUsers.update((users) => users.filter((u) => u !== username));
    });

    this.hubConnection.on('GetOnlineUsers', (currentUsers) => {
      this.onlineUsers.set(currentUsers);
    });

    this.hubConnection.on('NewMessageReceived', ({ username, knownAs }) => {
      this.toastr
        .info(knownAs + ' has sent you a message! Click me to see it.')
        .onTap.pipe(take(1))
        .subscribe(() =>
          this.router.navigateByUrl('/members/' + username + '?tab=Messages')
        );
    });
  }

  stopHubConnection() {
    if (this.hubConnection?.state === HubConnectionState.Connected) {
      this.hubConnection.stop().catch((e) => console.error(e));
    }
  }
}
