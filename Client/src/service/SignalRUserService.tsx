import * as signalR from '@microsoft/signalr';
import { CommentDTO } from '../props/EntitiesDTO';

class SignalRUserService {
  private connection: signalR.HubConnection | undefined;

  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5164/UserHub')
      .withAutomaticReconnect()
      .build();

    this.connection.start().catch(error => console.error(error));
  }

  public getConnection() {
    return this.connection;
  }

  public BlockUser(usersIds: string[]) {
    if (this.connection && usersIds) {
        this.connection.invoke('BlockUser', usersIds);
    }
  }

  public UnblockUser(usersIds: string[]) {
    if (this.connection && usersIds) {
        this.connection.invoke('UnblockUser', usersIds);
    }
  }

  public MakeUserAdmin(usersIds: string[]) {
    if (this.connection && usersIds) {
        this.connection.invoke('MakeUserAdmin', usersIds);
    }
  }
}

const signalRUserService = new SignalRUserService();

export default signalRUserService;