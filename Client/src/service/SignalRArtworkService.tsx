import * as signalR from '@microsoft/signalr';
import { CommentDTO } from '../props/EntitiesDTO';

class SignalRArtworkService {
  private connection: signalR.HubConnection | undefined;

  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('http://peabody28.com:1032/ArtworkHub')
      .withAutomaticReconnect()
      .build();

    this.connection.start().catch(error => console.error(error));
  }

  public getConnection() {
    return this.connection;
  }

  public RateArtwork(artworkId: string, loggedInUserId: string, rateValue: number | null) {
    if (this.connection && artworkId && loggedInUserId && rateValue) {
        this.connection.invoke('RateArtwork', artworkId, loggedInUserId, rateValue);
    }
  }
}

const signalRArtworkService = new SignalRArtworkService();

export default signalRArtworkService;