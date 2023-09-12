import * as signalR from '@microsoft/signalr';
import { CommentDTO } from './EntitiesDTO';

class SignalRCommentService {
  private connection: signalR.HubConnection | undefined;

  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('http://localhost:5164/CommentHub')
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
}

const signalRCommentService = new SignalRCommentService();

export default signalRCommentService;