export interface CommentDTO {
    text: string;
    reviewId: string;
    userId: string;
}

export interface ReviewDTO {
    title: string;
    text: string;
    mark: number | undefined;
    artworkName: string;
    groupId: number;
    userId: string | undefined;
    error: string;
}
