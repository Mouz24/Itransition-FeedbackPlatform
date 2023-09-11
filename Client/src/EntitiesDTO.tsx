export interface CommentDTO {
    text: string;
    reviewId: string | undefined;
    userId: string | undefined;
}

export interface ReviewDTO {
    title: string;
    text: string;
    mark: number | undefined;
    artworkName: string;
    groupId: number | undefined;
    userId: string | undefined;
    error: string;
}
