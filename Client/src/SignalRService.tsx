import * as signalR from '@microsoft/signalr';
import { CommentDTO } from './EntitiesDTO';

class SignalRService {
  private connection: signalR.HubConnection | undefined;

  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5164/Hub')
      .withAutomaticReconnect()
      .build();

    this.connection.start().catch(error => console.error(error));
  }

  public getConnection() {
    return this.connection;
  }

  public LeaveComment(comment: CommentDTO) {
    if (this.connection && comment.text && comment.reviewId && comment.userId) {
        this.connection.invoke('LeaveComment', comment);
    }
  }

  public RemoveComment(id: string) {
    if (this.connection && id) {
        this.connection.invoke('RemoveComment', id);
    }
  }

  public LikeReview(reviewId: string, loggedInUserId: string) {
    if (this.connection && reviewId && loggedInUserId) {
        this.connection.invoke('LikeReview', reviewId, loggedInUserId);
    }
  }

  public RateArtwork(artworkId: string, loggedInUserId: string, rateValue: number | null) {
    if (this.connection && artworkId && loggedInUserId && rateValue) {
        this.connection.invoke('RateArtwork', artworkId, loggedInUserId, rateValue);
    }
  }
}

const signalRService = new SignalRService();

export default signalRService;