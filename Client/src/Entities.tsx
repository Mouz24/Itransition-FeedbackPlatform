export interface Review {
    id: string;
    title: string;
    text: string;
    dateCreated: string;
    mark: number;
    likes: number;
    isLikedByUser: boolean;
    artwork: Artwork;
    group: Group;
    user: User;
    reviewImages: ReviewImage[];
    comments: Comment[];
}

export interface Artwork {
    id: string;
    name: string;
    rate: number;   
}

export interface Group {
    id: number;
    name: string;
}

export interface User {
    id: string;
    userName: string;
    avatar: string;
    likes: number;
}

export interface Comment {
    id: string;
    text: string;
    dateCreated: string;
    user: User;
}

export interface ReviewImage {
    imageUrl: string;
} 

export interface Tag {
    id: number;
    text: string;
}