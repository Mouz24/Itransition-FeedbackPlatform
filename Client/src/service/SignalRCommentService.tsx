import * as signalR from '@microsoft/signalr';
import { CommentDTO } from '../props/EntitiesDTO';

class SignalRCommentService {
  private connection: signalR.HubConnection | undefined;

  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('http://peabody28.com:1032/CommentHub')
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

  public RemoveComment(id: string, reviewId: string) {
    if (this.connection && id && reviewId) {
        this.connection.invoke('RemoveComment', id, reviewId);
    }
  }
}

const signalRCommentService = new SignalRCommentService();

export default signalRCommentService;