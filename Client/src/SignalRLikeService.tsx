import * as signalR from '@microsoft/signalr';

class SignalRLikeService {
  private connection: signalR.HubConnection | undefined;

  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
    .withUrl('http://localhost:5164/LikeHub')
      .withAutomaticReconnect()
      .build();

    this.connection.start().catch(error => console.error(error));
  }

  public getConnection() {
    return this.connection;
  }

  public LikeReview(reviewId: string, userId: string ) {
    if (this.connection && reviewId && userId) {
        this.connection.invoke('LikeReview', reviewId, userId);
    }
  }
}

const signalRLikeService = new SignalRLikeService();

export default signalRLikeService;