import axios, { isAxiosError } from 'axios';
import React, { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import axiosInstance from './AxiosInstance';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import { useUserContext } from './UserContext';
import { ReviewDTO } from './EntitiesDTO';
import FormControl from '@mui/material/FormControl';
import InputLabel from '@mui/material/InputLabel';
import Select, { SelectChangeEvent } from '@mui/material/Select';
import MenuItem from '@mui/material/MenuItem';
import { Artwork, Group, Review, Tag } from './Entities';
import { useDropzone } from 'react-dropzone';
import ReactQuill from 'react-quill';
import 'react-quill/dist/quill.snow.css';
import 'react-quill/dist/quill.bubble.css';
import { styled } from '@mui/system';
import { error } from 'console';
import { Chip, CircularProgress, Divider, FormHelperText } from '@mui/material';
import Autocomplete from '@mui/material/Autocomplete';;

const DropzoneContainer = styled('div')({
  border: '2px dashed #cccccc',
  borderRadius: '4px',
  padding: '20px',
  textAlign: 'center',
  cursor: 'pointer',
  marginBottom: '10px',
  '& p': {
    margin: '0',
  },
  '&.active': {
    borderColor: '#007bff',
  },
});

const EditReview: React.FC = () => {
  const navigate = useNavigate();
  const { setLoggedInUser } = useUserContext();
  const { reviewId } = useParams<{ reviewId: string }>();
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const { userId } = useParams<{ userId: string }>();
  const [groupOptions, setGroupOptions] = useState<Group[]>([]);
  const [mark, setMark] = useState('');
  const [availableTags, setAvailableTags] = useState<Tag[]>([]);
  const [artworks, setArtworks] = useState<Artwork[]>([]);
  const [group, setGroup] = useState('');
  const [addedTags, setAddedTags] = useState<string[]>([]);
  const [selectedArtwork, setSelectedArtwork] = useState<string | null>(null);
  const [imageFiles, setImageFiles] = useState<File[]>([]);
  const [newTag, setNewTag] = useState('');
  const[isTagBeingAdded, setIsTagBeingAdded] = useState<boolean>(false);
  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    accept: {
      'image/*': []
    },
    onDrop: (acceptedFile) => {
      const newImageFiles = [...imageFiles, ...acceptedFile];
      setImageFiles(newImageFiles);
      console.log(imageFiles);
    }
  });

  const FilePreview = ({ files }: { files: File[] }) => (
    <Box sx={{ display: 'flex', flexWrap: 'wrap' }}>
      {files.map((file, index) => (
        <div key={index} style={{ position: 'relative' }}>
          <img
            src={URL.createObjectURL(file)}
            alt={`File ${index}`}
            width="100"
            height="100"
          />
          <Chip
          label="Delete"
          onClick={() => handleRemoveImage(index)}
          color="secondary"
          style={{
            position: 'absolute',
            top: '5px',
            right: '5px',
            cursor: 'pointer',
          }}
        />
      </div>
      ))}
    </Box>
  );
  
  
  const handleRemoveImage = (indexToRemove: number) => {
    const newImageFiles = imageFiles.filter((_, index) => index !== indexToRemove);
    setImageFiles(newImageFiles);
  };

  const [formData, setFormData] = useState<ReviewDTO>({
    title: '',
    text: '',
    mark: null,
    artworkName: '',
    groupId: null,
    userId: userId,
    error: ''
  });

  const [fieldErrors, setFieldErrors] = useState({
    title: '',
    text: '',
    mark: '',
    artworkName: '',
    group: '',
  });

  useEffect(() => {
    fetchGroupOptions();
    fetchTags();
    fetchArtworks();
    fetchReviewData();
  }, []);

  const fetchArtworks = async () => {
    try {
      const response = await axiosInstance.get('artwork');
      setArtworks(response.data);
    } catch (error) {
      console.error('Error fetching group options:', error);
    }
  };

  const fetchGroupOptions = async () => {
    try {
      const response = await axiosInstance.get('group');
      setGroupOptions(response.data);
    } catch (error) {
      console.error('Error fetching group options:', error);
    }
  };

  const fetchTags = async () => {
    try {
      const response = await axiosInstance.get('tag');
      setAvailableTags(response.data);
    } catch (error) {
      console.error('Error fetching group options:', error);
    }
  };

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = event.target;
    setFormData((prevFormData) => ({
      ...prevFormData,
      [name]: value,
      error: '',
    }));

    setFieldErrors((prevFieldErrors) => ({
      ...prevFieldErrors,
      [name]: '',
    }));
  };

  const handleQuillChange = (value: string) => {
    setFormData((prevFormData) => ({
      ...prevFormData,
      text: value,
      error: '',
    }));
  };

  const handleMarkChange = (event: SelectChangeEvent) => {
    const value = Number(event.target.value);
    if (!isNaN(value) && value >= 1 && value <= 10) {
      setMark(event.target.value);
      setFormData((prevFormData) => ({
        ...prevFormData,
        mark: value,
        error: '',
      }));
    }
  };

  const handleGroupChange = (event: SelectChangeEvent) => {
    const groupName = event.target.value;
    const selectedGroup = groupOptions.find((group) => group.name === groupName);

    setGroup(groupName);

    if (selectedGroup) {
      setFormData((prevFormData) => ({
        ...prevFormData,
        groupId: selectedGroup.id,
        error: '',
      }));
    }
  };

  const handleRemoveTag = (tagToRemove : string) => {
    const updatedTags = addedTags.filter((tag) => tag !== tagToRemove);
    setAddedTags(updatedTags);
  };

  const handleAddTag = () => {
    setAddedTags([...addedTags, newTag])
    setIsTagBeingAdded(false);
    setNewTag('');
  };

  const fetchReviewData = async () => {
    try {
      const response = await axiosInstance.get<Review>(`review/${userId}/${reviewId}`);
      console.log(response.data);
      const reviewData = response.data;

      setFormData({
        title: reviewData.title,
        text: reviewData.text,
        mark: Number(reviewData.mark),
        artworkName: reviewData.artwork.name,
        groupId: reviewData.group.id,
        userId: userId,
        error: '',
      });

      setGroup(reviewData.group.name);
      setMark(String(reviewData.mark));
      setSelectedArtwork(reviewData.artwork.name);

      setAddedTags([...reviewData.tags.map((tag) => tag.value)]);
      
      const reviewImageFiles = reviewData.reviewImages.map((reviewImage) => {
        return new File([], reviewImage.imageUrl);
      });
  
      setImageFiles(reviewImageFiles);

    } catch (error) {
      console.error('Error fetching review data:', error);
    }
  };


  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();

    if (isLoading) return;

    setIsLoading(true);

    const quillContent = formData.text.trim();
    const quillContentWithoutTags = quillContent.replace(/<[^>]*>/g, ''); // Remove HTML tags
  
    if (quillContentWithoutTags === '') {
      setFieldErrors((prevFieldErrors) => ({
        ...prevFieldErrors,
        text: 'Please enter text for the review.',
      }));
      setIsLoading(false);
      return;
    }

    try {
      const formDataToSend = new FormData();

      formDataToSend.append('title', formData.title);
      formDataToSend.append('text', formData.text);
      formDataToSend.append('artworkName', formData.artworkName);
      if (formData.userId) {
        formDataToSend.append('userId', formData.userId);
      }
      if (formData.mark) {
        formDataToSend.append('mark', String(formData.mark));
      }
      if (formData.groupId) {
        formDataToSend.append('groupId', String(formData.groupId));
      }

      imageFiles.forEach((file) => {
        formDataToSend.append(`imageFiles`, file);
      });

      addedTags.forEach((tag) => {
        formDataToSend.append('tags', tag);
      })

      const response = await axiosInstance.put(`review/${userId}/${reviewId}`, formDataToSend, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      });

      setIsLoading(false);
      navigate(`/${userId}/reviews/${reviewId}`);
    } catch (error: any) {
      if (isAxiosError(error)) {
        if (error.response) {
          const responseData = error.response.data;
          if (responseData) {
            setFieldErrors({
              title: responseData.Title?.[0] || '',
              text: responseData.Text?.[0] || '',
              artworkName: responseData.ArtworkName?.[0] || '',
              mark: responseData.Mark?.[0] || '',
              group: responseData.GroupId?.[0] || '',
            });
            setIsLoading(false);
          }
        } else {
          console.error(
            'Unknown Error:',
            'An unknown error occurred during registration.'
          );
          setFormData((prevFormData) => ({
            ...prevFormData,
            error: 'An unknown error occurred during registration.',
          }));
        }
      }
    }
  };
  return (
    <Box
    component="form"
    sx={{
    '& .MuiTextField-root': { m: 1, width: '25ch' },
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    }}
    noValidate
    autoComplete="on"
    onSubmit={handleSubmit}
    >
    {isLoading && (
    <CircularProgress
    size={48}
    thickness={5}
    sx={{ marginBottom: '10px' }}
    />
    )}
    <div>
      <TextField
        label={fieldErrors.title ? 'Error' : 'Title'}
        variant="outlined"
        type="text"
        name="title"
        value={formData.title}
        onChange={handleChange}
        error={Boolean(fieldErrors.title)}
        helperText={fieldErrors.title || ''}
      />
      </div>
      <div>
      <Autocomplete
      freeSolo
      options={artworks.map((artwork) => artwork.name)}
      value={selectedArtwork}
      onChange={(_, newValue) => {
        setSelectedArtwork(newValue);
        setFormData((prevFormData) => ({
          ...prevFormData,
          artworkName: newValue || '',
        }));
      }}
        renderInput={(params) => (
        <TextField
        {...params}
        label={fieldErrors.artworkName ? 'Error' : 'Artwork'}
        variant="outlined"
        name="artworkName"
        value={selectedArtwork || formData.artworkName}
        onChange={(event) => {
            setSelectedArtwork(event.target.value);
            setFormData((prevFormData) => ({
            ...prevFormData,
            artworkName: event.target.value,
            }));
        }}
        error={Boolean(fieldErrors.artworkName)}
        helperText={fieldErrors.artworkName || ''}
        />
        )}
    />
    </div>
    <div>
        <ReactQuill
          value={formData.text}
          onChange={handleQuillChange}
          modules={{
            toolbar: [
              [{ header: '1' }, { header: '2' }, { font: [] }],
              ['bold', 'italic', 'underline', 'strike', 'blockquote'],
              [{ color: [] }],
              [{ list: 'ordered' }, { list: 'bullet' }],
              ['clean'],
              ["blockquote", "code-block"],
            ],
          }}
        />
    </div>
    {fieldErrors.text && (
    <div style={{ color: 'red', marginTop: '5px' }}>
        {fieldErrors.text}
    </div>
    )}
    <div>
      <FormControl sx={{width: '100px', marginTop: '10px'}} 
        error={Boolean(fieldErrors.mark)}
      >
        <InputLabel htmlFor="mark">Mark</InputLabel>
        <Select
          id="mark"
          name="mark"
          value={mark}
          onChange={handleMarkChange}
          variant="outlined"
        >
          {Array.from({ length: 10 }, (_, i) => i + 1).map((value) => (
            <MenuItem key={value} value={String(value)}>
              {value}
            </MenuItem>
          ))}
        </Select>
        <FormHelperText>{fieldErrors.mark}</FormHelperText>
      </FormControl>
    </div>
    <div>
      <FormControl sx={{width: '100px'}}
        error={Boolean(fieldErrors.group)}
      >
        <InputLabel htmlFor="group">Group</InputLabel>
        <Select
          id="group"
          name="group"
          value={group}
          onChange={handleGroupChange}
          variant="outlined"
        >
          {groupOptions.map((groupOption, index) => (
            <MenuItem key={index} value={groupOption.name}>
              {groupOption.name}
            </MenuItem>
          ))}
        </Select>
        <FormHelperText>{fieldErrors.group}</FormHelperText>
      </FormControl>
    </div>
    <Box sx={{display: 'flex', flexDirection: 'column', marginTop: '10px', marginBottom: '10px'}}>
      {!isTagBeingAdded && 
      <Button 
      variant='outlined' 
      sx={{borderRadius: '9px'}}
      color='inherit'
      onClick = {(() => {
        setIsTagBeingAdded(true);
      })}
      >
        Add tag
      </Button>
      }
      {isTagBeingAdded && 
      <Box sx={{display: 'flex'}}>
      <Autocomplete
        freeSolo
        options={availableTags.map(tag => tag.value)}
        value={newTag}
        onInputChange={(event, newValue) => setNewTag(newValue)}
        renderInput={params => (
          <TextField
            {...params}
            type="text"
            placeholder="Add new tag"
            size="small"
          />
        )}
      />
      <Button onClick={() => handleAddTag()}>Add</Button>
      </Box>
      }
      <Box sx={{display: 'flex', gap: '7px', marginTop: '7px'}}>
      {addedTags.map((tag) => (
        <Chip
        label={tag}
        onDelete={() => handleRemoveTag(tag)}
        color='default'
        variant='filled'
        />
      ))}
      </Box>
    </Box>
    <DropzoneContainer
      {...getRootProps()}
      className={isDragActive ? 'active' : ''}
    >
      <input {...getInputProps()} />
      {isDragActive ? (
        <p>Drop the image here...</p>
      ) : (
        <p>Drag &amp; drop an image here, or click to select one</p>
      )}
    </DropzoneContainer>
    <div>
        <FilePreview files={imageFiles}/>
    </div>
    <div>
      <Button variant="contained" color="success" type="submit" sx={{marginTop: '10px'}}>
        Edit
      </Button>
    </div>
  </Box>
  );
};

export default EditReview;
