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

  public LikeReview(reviewId: string | undefined, userId: string | undefined, loggedInUserId: string | undefined ) {
    if (this.connection && reviewId && userId && loggedInUserId) {
        this.connection.invoke('LikeReview', reviewId, userId, loggedInUserId);
    }
  }
}

const signalRLikeService = new SignalRLikeService();

export default signalRLikeService;